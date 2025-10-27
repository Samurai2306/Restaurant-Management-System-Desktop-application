using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.UI.ViewModels.Tables;
using RestaurantSystem.UI.ViewModels.Reservations;
using RestaurantSystem.UI.ViewModels.Menu;
using RestaurantSystem.UI.ViewModels.Orders;
using System.Windows.Input;
using RestaurantSystem.Core.Interfaces.Repositories;

namespace RestaurantSystem.UI.ViewModels;

public partial class MainWindowViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _title = "Система Управления Рестораном";

    [ObservableProperty]
    private BaseViewModel _currentView;

    [ObservableProperty]
    private bool _isMenuExpanded = true;

    [ObservableProperty]
    private string _userName = "Admin"; // Will be replaced with actual user

    private readonly TablesViewModel _tablesViewModel;
    private readonly ReservationsViewModel _reservationsViewModel;
    private readonly MenuViewModel _menuViewModel;
    private readonly OrdersViewModel _ordersViewModel;
    private readonly AnalyticsViewModel _analyticsViewModel;

    public ICommand NavigateTablesCommand { get; }
    public ICommand NavigateReservationsCommand { get; }
    public ICommand NavigateMenuCommand { get; }
    public ICommand NavigateOrdersCommand { get; }
    public ICommand NavigateAnalyticsCommand { get; }
    public ICommand ToggleMenuCommand { get; }

    public MainWindowViewModel(
        TablesViewModel tablesViewModel,
        ReservationsViewModel reservationsViewModel,
        MenuViewModel menuViewModel,
        OrdersViewModel ordersViewModel,
        AnalyticsViewModel analyticsViewModel)
    {
        _tablesViewModel = tablesViewModel;
        _reservationsViewModel = reservationsViewModel;
        _menuViewModel = menuViewModel;
        _ordersViewModel = ordersViewModel;
        _analyticsViewModel = analyticsViewModel;

        NavigateTablesCommand = new RelayCommand(() => CurrentView = _tablesViewModel);
        NavigateReservationsCommand = new RelayCommand(() => CurrentView = _reservationsViewModel);
        NavigateMenuCommand = new RelayCommand(() => CurrentView = _menuViewModel);
        NavigateOrdersCommand = new RelayCommand(() => CurrentView = _ordersViewModel);
        NavigateAnalyticsCommand = new RelayCommand(() => CurrentView = _analyticsViewModel);
        ToggleMenuCommand = new RelayCommand(() => IsMenuExpanded = !IsMenuExpanded);

        // Set default view
        CurrentView = _tablesViewModel;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(CurrentView))
        {
            Title = $"{CurrentView?.Title} - Система Управления Рестораном";
        }
    }
}