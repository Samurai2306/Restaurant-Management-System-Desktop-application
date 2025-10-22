using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RestaurantSystem.UI.ViewModels.Reservations;

public partial class ReservationsViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<ReservationViewModel> _reservations;

    [ObservableProperty]
    private ReservationViewModel _selectedReservation;

    [ObservableProperty]
    private DateTime _selectedDate;

    [ObservableProperty]
    private int? _selectedTableId;

 public ICommand RefreshCommand { get; }
    public ICommand AddReservationCommand { get; }
    public ICommand DateChangedCommand { get; }
    public ICommand TableChangedCommand { get; }

    public ReservationsViewModel()
    {
        Title = "Reservations Management";
 Reservations = new ObservableCollection<ReservationViewModel>();
  SelectedDate = DateTime.Today;

        RefreshCommand = new AsyncRelayCommand(LoadReservationsAsync);
  AddReservationCommand = new RelayCommand(OnAddReservation);
        DateChangedCommand = new AsyncRelayCommand(OnDateChanged);
        TableChangedCommand = new AsyncRelayCommand<int>(OnTableChanged);

        // Initial load
        LoadReservationsAsync().ConfigureAwait(false);
    }

    private async Task LoadReservationsAsync()
    {
        await ExecuteAsync(async () =>
   {
    // Will be implemented when we add services
        Reservations.Clear();
            // Temporary test data
  Reservations.Add(new ReservationViewModel
     {
  Id = 1,
      ClientName = "John Doe",
                StartTime = DateTime.Now.AddHours(1),
      EndTime = DateTime.Now.AddHours(3),
           TableId = 1,
              TableName = "Table 1",
    Status = Core.Enums.ReservationStatus.Confirmed
            });
        });
    }

    private void OnAddReservation()
    {
        // Will be implemented later
    }

    private async Task OnDateChanged()
    {
  await LoadReservationsAsync();
    }

    private async Task OnTableChanged(int tableId)
  {
        SelectedTableId = tableId;
        await LoadReservationsAsync();
    }
}