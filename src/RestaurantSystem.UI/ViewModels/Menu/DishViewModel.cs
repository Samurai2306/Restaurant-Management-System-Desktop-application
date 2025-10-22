using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.UI.ViewModels.Menu;

public partial class DishViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private decimal _price;

    [ObservableProperty]
    private DishCategory _category;

    [ObservableProperty]
    private int _cookingTimeMinutes;

    [ObservableProperty]
    private bool _isAvailable;

    [ObservableProperty]
    private string _imagePath;

    [ObservableProperty]
    private ObservableCollection<string> _tags;

    [ObservableProperty]
    private Allergen _allergens;

    public ICommand EditCommand { get; }
    public ICommand ToggleAvailabilityCommand { get; }
    public ICommand DeleteCommand { get; }

    public DishViewModel()
    {
        Tags = new ObservableCollection<string>();
      EditCommand = new RelayCommand(OnEdit);
        ToggleAvailabilityCommand = new RelayCommand(OnToggleAvailability);
        DeleteCommand = new RelayCommand(OnDelete);
    }

    private void OnEdit()
    {
        // Will be implemented later
    }

    private void OnToggleAvailability()
    {
     IsAvailable = !IsAvailable;
    }

    private void OnDelete()
  {
        // Will be implemented later
 }
}