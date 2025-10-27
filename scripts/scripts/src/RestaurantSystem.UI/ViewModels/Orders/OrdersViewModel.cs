using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.UI.ViewModels.Orders;

public partial class OrdersViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<OrderViewModel> _orders;

    [ObservableProperty]
    private OrderViewModel _selectedOrder;

    [ObservableProperty]
    private OrderStatus? _selectedStatus;

    [ObservableProperty]
    private int? _selectedTableId;

    [ObservableProperty]
    private DateTime? _selectedDate;

    public ICommand RefreshCommand { get; }
    public ICommand AddOrderCommand { get; }
    public ICommand FilterByStatusCommand { get; }
    public ICommand FilterByTableCommand { get; }
    public ICommand FilterByDateCommand { get; }

    public OrdersViewModel()
  {
        Title = "Orders Management";
      Orders = new ObservableCollection<OrderViewModel>();

        RefreshCommand = new AsyncRelayCommand(LoadOrdersAsync);
        AddOrderCommand = new RelayCommand(OnAddOrder);
   FilterByStatusCommand = new AsyncRelayCommand<OrderStatus>(OnFilterByStatus);
 FilterByTableCommand = new AsyncRelayCommand<int>(OnFilterByTable);
        FilterByDateCommand = new AsyncRelayCommand(OnFilterByDate);

        // Initial load
        LoadOrdersAsync().ConfigureAwait(false);
  }

  private async Task LoadOrdersAsync()
  {
 await ExecuteAsync(async () =>
     {
            // Will be implemented when we add services
     Orders.Clear();
   // Temporary test data
     Orders.Add(new OrderViewModel
      {
    Id = 1,
       TableId = 1,
     TableName = "Table 1",
CreatedTime = DateTime.Now.AddMinutes(-30),
      Status = OrderStatus.InProgress,
      Items = new ObservableCollection<OrderItemViewModel>
        {
             new() { DishName = "Caesar Salad", Quantity = 2, UnitPrice = 12.99m }
         }
            });
     });
    }

  private void OnAddOrder()
    {
        // Will be implemented later
    }

    private async Task OnFilterByStatus(OrderStatus status)
    {
        SelectedStatus = status;
        await LoadOrdersAsync();
    }

    private async Task OnFilterByTable(int tableId)
    {
      SelectedTableId = tableId;
        await LoadOrdersAsync();
    }

    private async Task OnFilterByDate()
  {
        await LoadOrdersAsync();
 }
}