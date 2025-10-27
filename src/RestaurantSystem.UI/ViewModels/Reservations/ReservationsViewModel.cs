using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.UI.Services;

namespace RestaurantSystem.UI.ViewModels.Reservations;

public partial class ReservationsViewModel : BaseViewModel
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<ReservationViewModel> _reservations = new();

    [ObservableProperty]
    private ReservationViewModel? _selectedReservation;

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private int? _selectedTableId;

    public ICommand RefreshCommand { get; }
    public ICommand AddReservationCommand { get; }
    public ICommand EditReservationCommand { get; }
    public ICommand CancelReservationCommand { get; }
    public ICommand DateChangedCommand { get; }

    public ReservationsViewModel(IReservationRepository reservationRepository, IDialogService dialogService)
    {
        _reservationRepository = reservationRepository;
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
                foreach (var reservation in result.Value)
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

    private void OnAddReservation()
    {
        _dialogService.ShowInformation("Add Reservation dialog will be implemented", "Add Reservation");
    }

    private void OnEditReservation()
    {
        if (SelectedReservation == null) return;
        _dialogService.ShowInformation($"Edit reservation for '{SelectedReservation.ClientName}'", "Edit Reservation");
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