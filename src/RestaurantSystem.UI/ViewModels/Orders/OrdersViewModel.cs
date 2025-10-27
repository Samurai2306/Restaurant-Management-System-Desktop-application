using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.UI.Services;

namespace RestaurantSystem.UI.ViewModels.Orders;

public partial class OrdersViewModel : BaseViewModel
{
    private readonly IOrderRepository _orderRepository;
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

    public ICommand RefreshCommand { get; }
    public ICommand AddOrderCommand { get; }
    public ICommand ViewOrderCommand { get; }
    public ICommand CloseOrderCommand { get; }
    public ICommand FilterByStatusCommand { get; }

    public OrdersViewModel(IOrderRepository orderRepository, IDialogService dialogService)
    {
        _orderRepository = orderRepository;
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
                : await _orderRepository.GetActiveOrdersAsync();
            
            Orders.Clear();
            
            if (result.Succeeded)
            {
                foreach (var order in result.Value)
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

    private void OnAddOrder()
    {
        _dialogService.ShowInformation("New Order dialog will be implemented", "New Order");
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