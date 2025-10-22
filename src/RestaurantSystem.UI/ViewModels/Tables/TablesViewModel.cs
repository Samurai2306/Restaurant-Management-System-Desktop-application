using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.UI.ViewModels.Tables;

public partial class TablesViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<TableViewModel> _tables;

    [ObservableProperty]
    private TableViewModel _selectedTable;

    [ObservableProperty]
    private TableLocation? _selectedLocation;

    [ObservableProperty]
    private string _searchText;

    public ICommand RefreshCommand { get; }
    public ICommand AddTableCommand { get; }
    public ICommand FilterByLocationCommand { get; }
    public ICommand SearchCommand { get; }

    public TablesViewModel()
    {
        Title = "Tables Management";
        Tables = new ObservableCollection<TableViewModel>();

        RefreshCommand = new AsyncRelayCommand(LoadTablesAsync);
  AddTableCommand = new RelayCommand(OnAddTable);
        FilterByLocationCommand = new AsyncRelayCommand<TableLocation>(OnFilterByLocation);
        SearchCommand = new AsyncRelayCommand(OnSearch);

     // Initial load
        LoadTablesAsync().ConfigureAwait(false);
    }

    private async Task LoadTablesAsync()
    {
        await ExecuteAsync(async () =>
        {
     // Will be implemented when we add services
       Tables.Clear();
            // Temporary test data
    Tables.Add(new TableViewModel { Id = 1, Name = "Table 1", Location = TableLocation.MainHall, SeatsCount = 4, Status = TableStatus.Available });
       Tables.Add(new TableViewModel { Id = 2, Name = "Table 2", Location = TableLocation.Window, SeatsCount = 2, Status = TableStatus.Reserved });
        });
    }

    private void OnAddTable()
    {
        // Will be implemented later
    }

    private async Task OnFilterByLocation(TableLocation location)
 {
 SelectedLocation = location;
await LoadTablesAsync();
    }

    private async Task OnSearch()
    {
        await LoadTablesAsync();
    }
}