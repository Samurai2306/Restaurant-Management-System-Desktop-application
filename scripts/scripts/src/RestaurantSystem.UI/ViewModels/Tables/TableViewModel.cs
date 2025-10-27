using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.UI.ViewModels.Tables;

public partial class TableViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private TableLocation _location;

    [ObservableProperty]
    private int _seatsCount;

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private TableStatus _status;

    [ObservableProperty]
    private bool _isSelected;

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CreateReservationCommand { get; }

    public TableViewModel()
    {
        EditCommand = new RelayCommand(OnEdit);
   DeleteCommand = new RelayCommand(OnDelete);
   CreateReservationCommand = new RelayCommand(OnCreateReservation);
    }

    private void OnEdit()
    {
        // Will be implemented later
    }

    private void OnDelete()
    {
        // Will be implemented later
    }

    private void OnCreateReservation()
    {
// Will be implemented later
    }
}