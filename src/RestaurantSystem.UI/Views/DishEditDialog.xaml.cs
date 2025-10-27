using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.UI.Views;

public partial class DishEditDialog : Window
{
    public string DishName { get; set; } = string.Empty;
    public DishCategory Category { get; set; } = DishCategory.MainCourse;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0m;
    public int CookingTime { get; set; } = 30;
    public bool IsAvailable { get; set; } = true;
    public int? DishId { get; set; }

    public DishEditDialog()
    {
        InitializeComponent();
    }

    public DishEditDialog(Dish dish) : this()
    {
        Title = "Edit Dish";
        TitleTextBlock.Text = "Edit Dish";
        DishId = dish.Id;
        DishName = dish.Name;
        Category = dish.Category;
        Description = dish.Description;
        Price = dish.Price;
        CookingTime = dish.CookingTimeMinutes;
        IsAvailable = dish.IsAvailable;
        
        // Set fields after loading
        Loaded += (s, e) =>
        {
            NameTextBox.Text = DishName;
            DescriptionTextBox.Text = Description;
            PriceTextBox.Text = Price.ToString("F2");
            CookingTimeTextBox.Text = CookingTime.ToString();
            IsAvailableCheckBox.IsChecked = IsAvailable;
            
            // Select category
            var catItem = CategoryComboBox.Items.OfType<ComboBoxItem>()
                .FirstOrDefault(i => i.Tag?.ToString() == Category.ToString());
            if (catItem != null) CategoryComboBox.SelectedItem = catItem;
        };
    }

    private bool IsValidPrice(string priceText, out decimal price)
    {
        price = 0;
        if (!decimal.TryParse(priceText, out price)) return false;
        return price >= 0.01m && price <= 10000m;
    }

    private bool IsValidCookingTime(string timeText, out int time)
    {
        time = 0;
        if (!int.TryParse(timeText, out time)) return false;
        return time >= 1 && time <= 180;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            MessageBox.Show("Dish name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            NameTextBox.Focus();
            return;
        }

        if (NameTextBox.Text.Length > 100)
        {
            MessageBox.Show("Dish name cannot exceed 100 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            NameTextBox.Focus();
            return;
        }

        if (!IsValidPrice(PriceTextBox.Text, out var price))
        {
            MessageBox.Show("Please enter a valid price between $0.01 and $10,000.", 
                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            PriceTextBox.Focus();
            return;
        }

        if (!IsValidCookingTime(CookingTimeTextBox.Text, out var cookingTime))
        {
            MessageBox.Show("Please enter a valid cooking time between 1 and 180 minutes.", 
                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            CookingTimeTextBox.Focus();
            return;
        }

        // Get values
        DishName = NameTextBox.Text.Trim();
        Description = DescriptionTextBox.Text?.Trim() ?? string.Empty;
        Price = price;
        CookingTime = cookingTime;
        IsAvailable = IsAvailableCheckBox.IsChecked ?? true;
        
        if (CategoryComboBox.SelectedItem is ComboBoxItem catItem && 
            Enum.TryParse<DishCategory>(catItem.Tag?.ToString(), out var category))
        {
            Category = category;
        }
        
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public Dish GetDish()
    {
        var dish = new Dish
        {
            Name = DishName,
            Description = Description,
            Price = Price,
            Category = Category,
            CookingTimeMinutes = CookingTime,
            IsAvailable = IsAvailable,
            ImagePath = string.Empty
        };

        if (DishId.HasValue)
        {
            dish.Id = DishId.Value;
        }

        return dish;
    }
}
