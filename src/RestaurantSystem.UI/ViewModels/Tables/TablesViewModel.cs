using System.Collections.ObjectModel;
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
    private TableLocation? _selectedLocation;

    [ObservableProperty]
    private string _searchText = string.Empty;

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
        FilterByLocationCommand = new AsyncRelayCommand<TableLocation?>(OnFilterByLocationAsync);
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
        });
    }

    private void OnAddTable()
    {
        _dialogService.ShowInformation("Add Table dialog will be implemented", "Add Table");
        // TODO: Show add table dialog
    }

    private void OnEditTable()
    {
        if (SelectedTable == null) return;
        
        _dialogService.ShowInformation($"Edit table '{SelectedTable.Name}'", "Edit Table");
        // TODO: Show edit table dialog
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

    private async Task OnFilterByLocationAsync(TableLocation? location)
    {
        SelectedLocation = location;

        if (location == null)
        {
            await LoadTablesAsync();
            return;
        }

        await ExecuteAsync(async () =>
        {
            var result = await _tableRepository.GetTablesByLocationAsync(location.Value);
            
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