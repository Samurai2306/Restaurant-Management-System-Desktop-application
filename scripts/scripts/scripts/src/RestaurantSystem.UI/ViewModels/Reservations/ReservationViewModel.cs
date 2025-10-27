using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.UI.ViewModels.Reservations;

public partial class ReservationViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _clientName;

    [ObservableProperty]
    private string _clientPhone;

    [ObservableProperty]
    private DateTime _startTime;

    [ObservableProperty]
    private DateTime _endTime;

    [ObservableProperty]
    private int _tableId;

    [ObservableProperty]
    private string _tableName;

    [ObservableProperty]
    private string _comment;

    [ObservableProperty]
    private ReservationStatus _status;

    public ICommand EditCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand CheckInCommand { get; }

    public ReservationViewModel()
    {
        EditCommand = new RelayCommand(OnEdit);
  CancelCommand = new RelayCommand(OnCancel);
        CheckInCommand = new RelayCommand(OnCheckIn);
    }

    private void OnEdit()
    {
 // Will be implemented later
    }

  private void OnCancel()
    {
        // Will be implemented later
    }

    private void OnCheckIn()
 {
        // Will be implemented later
    }
}