using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.UI.ViewModels.Menu;

public partial class MenuViewModel : BaseViewModel
{
  [ObservableProperty]
    private ObservableCollection<DishViewModel> _dishes;

    [ObservableProperty]
    private DishViewModel _selectedDish;

    [ObservableProperty]
    private DishCategory? _selectedCategory;

    [ObservableProperty]
    private string _searchText;

    [ObservableProperty]
    private ObservableCollection<string> _selectedTags;

    public ICommand RefreshCommand { get; }
    public ICommand AddDishCommand { get; }
    public ICommand FilterByCategoryCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand FilterByTagCommand { get; }

    public MenuViewModel()
    {
    Title = "Menu Management";
        Dishes = new ObservableCollection<DishViewModel>();
      SelectedTags = new ObservableCollection<string>();

        RefreshCommand = new AsyncRelayCommand(LoadDishesAsync);
AddDishCommand = new RelayCommand(OnAddDish);
        FilterByCategoryCommand = new AsyncRelayCommand<DishCategory>(OnFilterByCategory);
        SearchCommand = new AsyncRelayCommand(OnSearch);
        FilterByTagCommand = new AsyncRelayCommand<string>(OnFilterByTag);

        // Initial load
    LoadDishesAsync().ConfigureAwait(false);
 }

    private async Task LoadDishesAsync()
  {
    await ExecuteAsync(async () =>
  {
  // Will be implemented when we add services
      Dishes.Clear();
      // Temporary test data
  Dishes.Add(new DishViewModel
       {
          Id = 1,
       Name = "Caesar Salad",
       Description = "Fresh romaine lettuce with caesar dressing",
    Price = 12.99m,
    Category = DishCategory.Salad,
     CookingTimeMinutes = 15,
                IsAvailable = true
  });
  });
    }

    private void OnAddDish()
    {
        // Will be implemented later
    }

    private async Task OnFilterByCategory(DishCategory category)
{
  SelectedCategory = category;
        await LoadDishesAsync();
    }

    private async Task OnSearch()
    {
        await LoadDishesAsync();
    }

    private async Task OnFilterByTag(string tag)
    {
        if (SelectedTags.Contains(tag))
            SelectedTags.Remove(tag);
     else
         SelectedTags.Add(tag);

        await LoadDishesAsync();
    }
}