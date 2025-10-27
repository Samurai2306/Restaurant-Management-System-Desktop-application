using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.UI.Services;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.UI.ViewModels.Orders;

public partial class OrdersViewModel : BaseViewModel
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<OrderViewModel> _orders = new();

    [ObservableProperty]
    private OrderViewModel? _selectedOrder;

    [ObservableProperty]
    private OrderStatus? _selectedStatus;

    [ObservableProperty]
    private int? _selectedTableId;

    [ObservableProperty]
    private DateTime? _selectedDate;
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    partial void OnSearchTextChanged(string? value)
    {
        _ = LoadOrdersAsync();
    }
    
    partial void OnSelectedStatusChanged(OrderStatus? value)
    {
        _ = LoadOrdersAsync();
    }
    
    partial void OnSelectedTableIdChanged(int? value)
    {
        _ = LoadOrdersAsync();
    }

    public ICommand RefreshCommand { get; }
    public ICommand AddOrderCommand { get; }
    public ICommand ViewOrderCommand { get; }
    public ICommand CloseOrderCommand { get; }
    public ICommand FilterByStatusCommand { get; }

    public OrdersViewModel(IOrderRepository orderRepository, ITableRepository tableRepository, IDialogService dialogService)
    {
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
        _dialogService = dialogService;
        
        Title = "Orders Management";

        RefreshCommand = new AsyncRelayCommand(LoadOrdersAsync);
        AddOrderCommand = new RelayCommand(OnAddOrder);
        ViewOrderCommand = new RelayCommand(OnViewOrder, () => SelectedOrder != null);
        CloseOrderCommand = new AsyncRelayCommand(OnCloseOrderAsync, () => SelectedOrder != null);
        FilterByStatusCommand = new AsyncRelayCommand<OrderStatus?>(OnFilterByStatusAsync);

        _ = LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = SelectedStatus.HasValue
                ? await _orderRepository.GetOrdersByStatusAsync(SelectedStatus.Value)
                : await _orderRepository.GetAllAsync();
            
            Orders.Clear();
            
            if (result.Succeeded)
            {
                var filtered = result.Value.AsQueryable();
                
                // Apply search filter
                if (!string.IsNullOrEmpty(SearchText))
                {
                    filtered = filtered.Where(o =>
                        (o.Table != null && !string.IsNullOrEmpty(o.Table.Name) && o.Table.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(o.SpecialInstructions) && o.SpecialInstructions.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
                }
                
                // Apply table filter
                if (SelectedTableId.HasValue)
                {
                    filtered = filtered.Where(o => o.TableId == SelectedTableId.Value);
                }
                
                foreach (var order in filtered)
                {
                    Orders.Add(new OrderViewModel
                    {
                        Id = order.Id,
                        TableId = order.TableId,
                        TableName = order.Table?.Name ?? "Unknown",
                        CreatedTime = order.CreatedTime,
                        Status = order.Status,
                        TotalAmount = order.TotalAmount,
                        ItemsCount = order.Items?.Count ?? 0,
                        SpecialInstructions = order.SpecialInstructions,
                        WaiterId = order.WaiterId
                    });
                }
            }
        });
    }

    private async void OnAddOrder()
    {
        try
        {
            // Get available tables
            var tablesResult = await _tableRepository.GetAllAsync();
            var tables = tablesResult.Succeeded ? tablesResult.Value.ToList() : new List<RestaurantSystem.Core.Models.Table>();
            
            var dialog = new Views.OrderEditDialog(tables);
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            
            var result = dialog.ShowDialog();
            if (result == true)
            {
                var order = dialog.GetOrder();
                var addResult = await _orderRepository.AddAsync(order);
                
                if (addResult.Succeeded)
                {
                    await _orderRepository.SaveChangesAsync();
                    _dialogService.ShowInformation("Order created successfully!", "Success");
                    await LoadOrdersAsync();
                }
                else
                {
                    _dialogService.ShowError(string.Join("\n", addResult.Errors), "Error");
                }
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Error creating order: {ex.Message}", "Error");
        }
    }

    private void OnViewOrder()
    {
        if (SelectedOrder == null) return;
        _dialogService.ShowInformation($"View order #{SelectedOrder.Id}", "Order Details");
    }

    private async Task OnCloseOrderAsync()
    {
        if (SelectedOrder == null) return;

        var confirmed = _dialogService.ShowConfirmation(
            $"Are you sure you want to close order #{SelectedOrder.Id}?",
            "Close Order");

        if (!confirmed) return;

        await ExecuteAsync(async () =>
        {
            var result = await _orderRepository.GetByIdAsync(SelectedOrder.Id);
            
            if (result.Succeeded && result.Value != null)
            {
                var order = result.Value;
                if (order.CanClose())
                {
                    order.Close();
                    await _orderRepository.SaveChangesAsync();
                    await LoadOrdersAsync();
                    _dialogService.ShowInformation("Order closed successfully", "Success");
                }
                else
                {
                    _dialogService.ShowWarning("Cannot close order. Check order status.", "Warning");
                }
            }
        });
    }

    private async Task OnFilterByStatusAsync(OrderStatus? status)
    {
        SelectedStatus = status;
        await LoadOrdersAsync();
    }

    partial void OnSelectedOrderChanged(OrderViewModel? value)
    {
        (ViewOrderCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (CloseOrderCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
    }
}