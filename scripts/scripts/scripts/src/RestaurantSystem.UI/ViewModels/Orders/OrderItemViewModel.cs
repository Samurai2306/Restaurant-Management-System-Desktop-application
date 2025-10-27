using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.UI.ViewModels.Orders;

public partial class OrderItemViewModel : ObservableObject
{
 [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private int _orderId;

    [ObservableProperty]
    private int _dishId;

    [ObservableProperty]
    private string _dishName;

    [ObservableProperty]
    private int _quantity;

    [ObservableProperty]
    private decimal _unitPrice;

    [ObservableProperty]
    private string _specialInstructions;

    [ObservableProperty]
    private OrderStatus _status;

    public decimal TotalPrice => Quantity * UnitPrice;

    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
 public ICommand RemoveCommand { get; }
    public ICommand UpdateStatusCommand { get; }

    public OrderItemViewModel()
    {
        IncreaseQuantityCommand = new RelayCommand(OnIncreaseQuantity);
     DecreaseQuantityCommand = new RelayCommand(OnDecreaseQuantity);
     RemoveCommand = new RelayCommand(OnRemove);
        UpdateStatusCommand = new RelayCommand<OrderStatus>(OnUpdateStatus);
    }

    private void OnIncreaseQuantity()
    {
        if (Quantity < 100) // Maximum limit
        Quantity++;
    }

  private void OnDecreaseQuantity()
    {
if (Quantity > 1) // Minimum limit
      Quantity--;
    }

    private void OnRemove()
    {
        // Will be implemented later
    }

    private void OnUpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
    }
}