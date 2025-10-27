using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.UI.ViewModels.Orders;

public partial class OrderViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private int _tableId;

    [ObservableProperty]
    private string _tableName;

    [ObservableProperty]
    private DateTime _createdTime;

    [ObservableProperty]
    private DateTime? _closedTime;

    [ObservableProperty]
    private string _specialInstructions;

    [ObservableProperty]
    private OrderStatus _status;

    [ObservableProperty]
    private string _waiterId;

    [ObservableProperty]
    private decimal _totalAmount;

    [ObservableProperty]
    private int _itemsCount;

    [ObservableProperty]
    private ObservableCollection<OrderItemViewModel> _items = new();

    public ICommand EditCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand CompleteCommand { get; }
    public ICommand AddItemCommand { get; }

    public OrderViewModel()
    {
        EditCommand = new RelayCommand(OnEdit);
        CancelCommand = new RelayCommand(OnCancel);
        CompleteCommand = new RelayCommand(OnComplete);
        AddItemCommand = new RelayCommand(OnAddItem);
    }

    private void OnEdit()
    {
// Will be implemented later
    }

    private void OnCancel()
    {
        // Will be implemented later
    }

    private void OnComplete()
    {
        // Will be implemented later
    }

    private void OnAddItem()
    {
        // Will be implemented later
    }
}