using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.UI.Services;

namespace RestaurantSystem.UI.ViewModels.Menu;

public partial class MenuViewModel : BaseViewModel
{
    private readonly IDishRepository _dishRepository;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private ObservableCollection<DishViewModel> _dishes = new();

    [ObservableProperty]
    private DishViewModel? _selectedDish;

    [ObservableProperty]
    private DishCategory? _selectedCategory;

    [ObservableProperty]
    private string _searchText = string.Empty;
    
    partial void OnSearchTextChanged(string? value)
    {
        _ = LoadDishesAsync();
    }
    
    partial void OnSelectedCategoryChanged(DishCategory? value)
    {
        _ = LoadDishesAsync();
    }

    [ObservableProperty]
    private ObservableCollection<string> _selectedTags = new();

    public ICommand RefreshCommand { get; }
    public ICommand AddDishCommand { get; }
    public ICommand EditDishCommand { get; }
    public ICommand DeleteDishCommand { get; }
    public ICommand FilterByCategoryCommand { get; }
    public ICommand SearchCommand { get; }

    public MenuViewModel(IDishRepository dishRepository, IDialogService dialogService)
    {
        _dishRepository = dishRepository;
        _dialogService = dialogService;
        
        Title = "Menu Management";

        RefreshCommand = new AsyncRelayCommand(LoadDishesAsync);
        AddDishCommand = new RelayCommand(OnAddDish);
        EditDishCommand = new RelayCommand(OnEditDish, () => SelectedDish != null);
        DeleteDishCommand = new AsyncRelayCommand(OnDeleteDishAsync, () => SelectedDish != null);
        FilterByCategoryCommand = new AsyncRelayCommand<DishCategory?>(OnFilterByCategoryAsync);
        SearchCommand = new AsyncRelayCommand(OnSearchAsync);

        _ = LoadDishesAsync();
    }

    private async Task LoadDishesAsync()
    {
        await ExecuteAsync(async () =>
        {
            var result = SelectedCategory.HasValue
                ? await _dishRepository.GetDishesByCategoryAsync(SelectedCategory.Value)
                : await _dishRepository.GetAllAsync();
            
            Dishes.Clear();
            
            if (result.Succeeded)
            {
                var dishes = result.Value;
                
                // Apply search filter
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    dishes = dishes.Where(d => 
                        d.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        d.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
                }

                foreach (var dish in dishes)
                {
                    Dishes.Add(new DishViewModel
                    {
                        Id = dish.Id,
                        Name = dish.Name,
                        Description = dish.Description,
                        Price = dish.Price,
                        Category = dish.Category,
                        CookingTimeMinutes = dish.CookingTimeMinutes,
                        IsAvailable = dish.IsAvailable,
                        ImagePath = dish.ImagePath
                    });
                }
            }
        });
    }

    private async void OnAddDish()
    {
        try
        {
            var dialog = new Views.DishEditDialog();
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            
            var result = dialog.ShowDialog();
            if (result == true)
            {
                var dish = dialog.GetDish();
                var addResult = await _dishRepository.AddAsync(dish);
                
                if (addResult.Succeeded)
                {
                    await _dishRepository.SaveChangesAsync();
                    _dialogService.ShowInformation("Dish added successfully!", "Success");
                    await LoadDishesAsync();
                }
                else
                {
                    _dialogService.ShowError(string.Join("\n", addResult.Errors), "Error");
                }
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Error adding dish: {ex.Message}", "Error");
        }
    }

    private void OnEditDish()
    {
        if (SelectedDish == null) return;
        _dialogService.ShowInformation($"Edit dish '{SelectedDish.Name}'", "Edit Dish");
    }

    private async Task OnDeleteDishAsync()
    {
        if (SelectedDish == null) return;

        var confirmed = _dialogService.ShowConfirmation(
            $"Are you sure you want to delete '{SelectedDish.Name}'?",
            "Delete Dish");

        if (!confirmed) return;

        await ExecuteAsync(async () =>
        {
            var result = await _dishRepository.DeleteAsync(SelectedDish.Id);
            
            if (result.Succeeded)
            {
                await _dishRepository.SaveChangesAsync();
                await LoadDishesAsync();
                _dialogService.ShowInformation("Dish deleted successfully", "Success");
            }
            else
            {
                _dialogService.ShowError(string.Join("\n", result.Errors), "Error");
            }
        });
    }

    private async Task OnFilterByCategoryAsync(DishCategory? category)
    {
        SelectedCategory = category;
        await LoadDishesAsync();
    }

    private async Task OnSearchAsync()
    {
        await LoadDishesAsync();
    }

    partial void OnSelectedDishChanged(DishViewModel? value)
    {
        (EditDishCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (DeleteDishCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
    }
}