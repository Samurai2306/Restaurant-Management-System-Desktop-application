using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using RestaurantSystem.Core.Interfaces.Repositories;

namespace RestaurantSystem.UI.ViewModels;

public partial class AnalyticsViewModel : BaseViewModel
{
    private readonly ITableRepository _tableRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IDishRepository _dishRepository;

    [ObservableProperty]
    private ObservableCollection<string> _recentActivity = new();

    [ObservableProperty]
    private int _totalTables;

    [ObservableProperty]
    private int _availableTables;

    [ObservableProperty]
    private int _occupiedTables;

    [ObservableProperty]
    private int _totalReservations;

    [ObservableProperty]
    private int _activeOrders;

    [ObservableProperty]
    private decimal _todayRevenue;

    [ObservableProperty]
    private int _totalDishes;

    [ObservableProperty]
    private int _availableDishes;

    public AnalyticsViewModel(
        ITableRepository tableRepository,
        IReservationRepository reservationRepository,
        IOrderRepository orderRepository,
        IDishRepository dishRepository)
    {
        _tableRepository = tableRepository;
        _reservationRepository = reservationRepository;
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
        
        Title = "Analytics & Dashboard";
        
        _ = LoadStatisticsAsync();
    }

    private async Task LoadStatisticsAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Load tables
            var tablesResult = await _tableRepository.GetAllAsync();
            if (tablesResult.Succeeded)
            {
                TotalTables = tablesResult.Value.Count();
                AvailableTables = tablesResult.Value.Count(t => t.IsActive);
                OccupiedTables = tablesResult.Value.Count(t => t.GetStatus(DateTime.Now) == Core.Enums.TableStatus.Occupied);
            }

            // Load reservations
            var reservationsResult = await _reservationRepository.GetReservationsByDateAsync(DateTime.Today);
            if (reservationsResult.Succeeded)
            {
                TotalReservations = reservationsResult.Value.Count();
            }

            // Load orders
            var ordersResult = await _orderRepository.GetActiveOrdersAsync();
            if (ordersResult.Succeeded)
            {
                ActiveOrders = ordersResult.Value.Count();
                
                // Calculate today revenue
                var allOrdersResult = await _orderRepository.GetOrdersByDateRangeAsync(DateTime.Today, DateTime.Today.AddDays(1));
                if (allOrdersResult.Succeeded)
                {
                    TodayRevenue = allOrdersResult.Value
                        .Where(o => o.Status == Core.Enums.OrderStatus.Paid)
                        .Sum(o => o.TotalAmount);
                }
            }

            // Load dishes
            var dishesResult = await _dishRepository.GetAllAsync();
            if (dishesResult.Succeeded)
            {
                TotalDishes = dishesResult.Value.Count();
                AvailableDishes = dishesResult.Value.Count(d => d.IsAvailable);
            }
        });
    }
}

