using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.Core.Interfaces;
using RestaurantSystem.UI.Services;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.UI.ViewModels.Reservations;

public partial class ReservationsViewModel : BaseViewModel
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<ReservationViewModel> _reservations = new();

    [ObservableProperty]
    private ReservationViewModel? _selectedReservation;

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private int? _selectedTableId;
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    partial void OnSearchTextChanged(string? value)
    {
        _ = LoadReservationsAsync();
    }

    public ICommand RefreshCommand { get; }
    public ICommand AddReservationCommand { get; }
    public ICommand EditReservationCommand { get; }
    public ICommand CancelReservationCommand { get; }
    public ICommand DateChangedCommand { get; }

    public ReservationsViewModel(IReservationRepository reservationRepository, ITableRepository tableRepository, IDialogService dialogService)
    {
        _reservationRepository = reservationRepository;
        _tableRepository = tableRepository;
        _dialogService = dialogService;
        
        Title = "Reservations Management";

        RefreshCommand = new AsyncRelayCommand(LoadReservationsAsync);
        AddReservationCommand = new RelayCommand(OnAddReservation);
        EditReservationCommand = new RelayCommand(OnEditReservation, () => SelectedReservation != null);
        CancelReservationCommand = new AsyncRelayCommand(OnCancelReservationAsync, () => SelectedReservation != null);
        DateChangedCommand = new AsyncRelayCommand(OnDateChangedAsync);

        _ = LoadReservationsAsync();
    }

    private async Task LoadReservationsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _reservationRepository.GetReservationsByDateAsync(SelectedDate);
            
            Reservations.Clear();
            
            if (result.Succeeded)
            {
                var filtered = result.Value.AsQueryable();
                
                // Apply search filter
                if (!string.IsNullOrEmpty(SearchText))
                {
                    filtered = filtered.Where(r =>
                        (!string.IsNullOrEmpty(r.ClientName) && r.ClientName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(r.ClientPhone) && r.ClientPhone.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (r.Table != null && !string.IsNullOrEmpty(r.Table.Name) && r.Table.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
                }
                
                // Apply table filter
                if (SelectedTableId.HasValue)
                {
                    filtered = filtered.Where(r => r.TableId == SelectedTableId.Value);
                }
                
                foreach (var reservation in filtered)
                {
                    Reservations.Add(new ReservationViewModel
                    {
                        Id = reservation.Id,
                        ClientName = reservation.ClientName,
                        ClientPhone = reservation.ClientPhone,
                        StartTime = reservation.StartTime,
                        EndTime = reservation.EndTime,
                        TableId = reservation.TableId,
                        TableName = reservation.Table?.Name ?? "Unknown",
                        Comment = reservation.Comment,
                        Status = reservation.Status
                    });
                }
            }
        });
    }

    private async void OnAddReservation()
    {
        try
        {
            // Get available tables
            var tablesResult = await _tableRepository.GetAllAsync();
            var tables = tablesResult.Succeeded ? tablesResult.Value.ToList() : new List<RestaurantSystem.Core.Models.Table>();
            
            var dialog = new Views.ReservationEditDialog(tables);
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            
            var result = dialog.ShowDialog();
            if (result == true)
            {
                var reservation = dialog.GetReservation();
                var addResult = await _reservationRepository.AddAsync(reservation);
                
                if (addResult.Succeeded)
                {
                    await _reservationRepository.SaveChangesAsync();
                    _dialogService.ShowInformation("Reservation added successfully!", "Success");
                    await LoadReservationsAsync();
                }
                else
                {
                    _dialogService.ShowError(string.Join("\n", addResult.Errors), "Error");
                }
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Error adding reservation: {ex.Message}", "Error");
        }
    }

    private async void OnEditReservation()
    {
        if (SelectedReservation == null) return;

        try
        {
            // Get full reservation entity
            var reservationResult = await _reservationRepository.GetByIdAsync(SelectedReservation.Id);
            if (!reservationResult.Succeeded || reservationResult.Value == null)
            {
                _dialogService.ShowError("Failed to load reservation details", "Error");
                return;
            }

            // Get available tables
            var tablesResult = await _tableRepository.GetAllAsync();
            var tables = tablesResult.Succeeded ? tablesResult.Value.ToList() : new List<Table>();

            var dialog = new Views.ReservationEditDialog(reservationResult.Value, tables);
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            
            var result = dialog.ShowDialog();
            if (result == true)
            {
                var updatedReservation = dialog.GetReservation();
                var updateResult = await _reservationRepository.UpdateAsync(updatedReservation);
                
                if (updateResult.Succeeded)
                {
                    await _reservationRepository.SaveChangesAsync();
                    _dialogService.ShowInformation("Reservation updated successfully!", "Success");
                    await LoadReservationsAsync();
                }
                else
                {
                    _dialogService.ShowError(string.Join("\n", updateResult.Errors), "Error");
                }
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Error editing reservation: {ex.Message}\n\n{ex.StackTrace}", "Error");
        }
    }

    private async Task OnCancelReservationAsync()
    {
        if (SelectedReservation == null) return;

        var confirmed = _dialogService.ShowConfirmation(
            $"Are you sure you want to cancel reservation for '{SelectedReservation.ClientName}'?",
            "Cancel Reservation");

        if (!confirmed) return;

        await ExecuteAsync(async () =>
        {
            var result = await _reservationRepository.DeleteAsync(SelectedReservation.Id);
            
            if (result.Succeeded)
            {
                await _reservationRepository.SaveChangesAsync();
                await LoadReservationsAsync();
                _dialogService.ShowInformation("Reservation cancelled successfully", "Success");
            }
            else
            {
                _dialogService.ShowError(string.Join("\n", result.Errors), "Error");
            }
        });
    }

    private async Task OnDateChangedAsync()
    {
        await LoadReservationsAsync();
    }

    partial void OnSelectedReservationChanged(ReservationViewModel? value)
    {
        (EditReservationCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (CancelReservationCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
    }
}