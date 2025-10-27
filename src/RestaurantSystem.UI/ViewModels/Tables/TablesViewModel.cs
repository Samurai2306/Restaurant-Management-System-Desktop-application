using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.UI.Services;

namespace RestaurantSystem.UI.ViewModels.Tables;

public partial class TablesViewModel : BaseViewModel
{
    private readonly ITableRepository _tableRepository;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<TableViewModel> _tables;

    [ObservableProperty]
    private TableViewModel? _selectedTable;

    [ObservableProperty]
    private string _selectedLocation = "";
    
    partial void OnSelectedLocationChanged(string? value)
    {
        _ = LoadTablesAsync();
    }

    [ObservableProperty]
    private string _searchText = string.Empty;
    
    partial void OnSearchTextChanged(string? value)
    {
        _ = LoadTablesAsync();
    }

    public ICommand RefreshCommand { get; }
    public ICommand AddTableCommand { get; }
    public ICommand EditTableCommand { get; }
    public ICommand DeleteTableCommand { get; }
    public ICommand FilterByLocationCommand { get; }
    public ICommand SearchCommand { get; }

    public TablesViewModel(ITableRepository tableRepository, IDialogService dialogService)
    {
        _tableRepository = tableRepository;
        _dialogService = dialogService;
        
        Title = "Tables Management";
        Tables = new ObservableCollection<TableViewModel>();

        RefreshCommand = new AsyncRelayCommand(LoadTablesAsync);
        AddTableCommand = new RelayCommand(OnAddTable);
        EditTableCommand = new RelayCommand(OnEditTable, () => SelectedTable != null);
        DeleteTableCommand = new AsyncRelayCommand(OnDeleteTableAsync, () => SelectedTable != null);
        FilterByLocationCommand = new AsyncRelayCommand<string?>(OnFilterByLocationAsync);
        SearchCommand = new AsyncRelayCommand(OnSearchAsync);

        // Initial load
        _ = LoadTablesAsync();
    }

    private async Task LoadTablesAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = await _tableRepository.GetAllAsync();
            
            Tables.Clear();
            
            if (result.Succeeded)
            {
                var filtered = result.Value.AsQueryable();
                
                // Apply location filter
                if (!string.IsNullOrEmpty(SelectedLocation))
                {
                    if (Enum.TryParse<TableLocation>(SelectedLocation, true, out var location))
                    {
                        filtered = filtered.Where(t => t.Location == location);
                    }
                }
                
                // Apply search filter
                if (!string.IsNullOrEmpty(SearchText))
                {
                    filtered = filtered.Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                }
                
                foreach (var table in filtered)
                {
                    Tables.Add(new TableViewModel
                    {
                        Id = table.Id,
                        Name = table.Name,
                        Location = table.Location,
                        SeatsCount = table.SeatsCount,
                        IsActive = table.IsActive,
                        Status = table.GetStatus(DateTime.Now)
                    });
                }
            }
        });
    }

    private async void OnAddTable()
    {
        try
        {
            var dialog = new Views.TableEditDialog();
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            
            var result = dialog.ShowDialog();
            if (result == true)
            {
                var table = dialog.GetTable();
                var addResult = await _tableRepository.AddAsync(table);
                
                if (addResult.Succeeded)
                {
                    await _tableRepository.SaveChangesAsync();
                    _dialogService.ShowInformation("Table added successfully!", "Success");
                    await LoadTablesAsync();
                }
                else
                {
                    _dialogService.ShowError(string.Join("\n", addResult.Errors), "Error");
                }
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Error adding table: {ex.Message}\n\n{ex.StackTrace}", "Error");
        }
    }

    private async void OnEditTable()
    {
        try
        {
            if (SelectedTable == null) return;

            // Get full table entity
            var tableResult = await _tableRepository.GetByIdAsync(SelectedTable.Id);
            if (!tableResult.Succeeded || tableResult.Value == null)
            {
                _dialogService.ShowError("Failed to load table details", "Error");
                return;
            }

            var dialog = new Views.TableEditDialog(tableResult.Value);
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            
            var result = dialog.ShowDialog();
            if (result == true)
            {
                var updatedTable = dialog.GetTable();
                var updateResult = await _tableRepository.UpdateAsync(updatedTable);
                
                if (updateResult.Succeeded)
                {
                    await _tableRepository.SaveChangesAsync();
                    _dialogService.ShowInformation("Table updated successfully!", "Success");
                    await LoadTablesAsync();
                }
                else
                {
                    _dialogService.ShowError(string.Join("\n", updateResult.Errors), "Error");
                }
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Error editing table: {ex.Message}\n\n{ex.StackTrace}", "Error");
        }
    }

    private async Task OnDeleteTableAsync()
    {
        if (SelectedTable == null) return;

        var confirmed = _dialogService.ShowConfirmation(
            $"Are you sure you want to delete table '{SelectedTable.Name}'?",
            "Delete Table");

        if (!confirmed) return;

        await ExecuteAsync(async () =>
        {
            var result = await _tableRepository.DeleteAsync(SelectedTable.Id);
            
            if (result.Succeeded)
            {
                await _tableRepository.SaveChangesAsync();
                await LoadTablesAsync();
                _dialogService.ShowInformation("Table deleted successfully", "Success");
            }
            else
            {
                _dialogService.ShowError(string.Join("\n", result.Errors), "Error");
            }
        });
    }

    private async Task OnFilterByLocationAsync(string? location)
    {
        SelectedLocation = location;

        if (location == null)
        {
            await LoadTablesAsync();
            return;
        }

        await ExecuteAsync(async () =>
        {
            if (Enum.TryParse<TableLocation>(SelectedLocation, true, out var locationEnum))
            {
                var result = await _tableRepository.GetTablesByLocationAsync(locationEnum);
                
                Tables.Clear();
                
                if (result.Succeeded)
                {
                    foreach (var table in result.Value)
                    {
                        Tables.Add(new TableViewModel
                        {
                            Id = table.Id,
                            Name = table.Name,
                            Location = table.Location,
                            SeatsCount = table.SeatsCount,
                            IsActive = table.IsActive,
                            Status = table.GetStatus(DateTime.Now)
                        });
                    }
                }
            }
        });
    }

    private async Task OnSearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadTablesAsync();
            return;
        }

        await ExecuteAsync(async () =>
        {
            var result = await _tableRepository.GetAllAsync();
            
            Tables.Clear();
            
            if (result.Succeeded)
            {
                var filtered = result.Value.Where(t => 
                    t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

                foreach (var table in filtered)
                {
                    Tables.Add(new TableViewModel
                    {
                        Id = table.Id,
                        Name = table.Name,
                        Location = table.Location,
                        SeatsCount = table.SeatsCount,
                        IsActive = table.IsActive,
                        Status = table.GetStatus(DateTime.Now)
                    });
                }
            }
        });
    }

    partial void OnSelectedTableChanged(TableViewModel? value)
    {
        (EditTableCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (DeleteTableCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
    }
}