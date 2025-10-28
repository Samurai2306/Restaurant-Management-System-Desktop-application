using System;
using System.Linq;
using System.Windows;
using RestaurantSystem.Core.Models;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.UI.Views;

public partial class OrderItemEditDialog : Window
{
    public int OrderId { get; private set; }
    public int? SelectedDishId { get; private set; }
    public int Quantity { get; private set; } = 1;
    public string SpecialInstructions { get; private set; } = string.Empty;

    private System.Collections.Generic.List<Table> _tables;
    private System.Collections.Generic.List<Dish> _dishes;

    public OrderItemEditDialog(int orderId, System.Collections.Generic.List<Dish> dishes)
    {
        InitializeComponent();

        OrderId = orderId;
        _dishes = dishes;

        // Populate dish combo box
        DishComboBox.ItemsSource = dishes;
        if (dishes.Any()) DishComboBox.SelectedIndex = 0;
    }

    private void DecreaseButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(QuantityTextBox.Text, out var qty) && qty > 1)
        {
            QuantityTextBox.Text = (qty - 1).ToString();
        }
    }

    private void IncreaseButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(QuantityTextBox.Text, out var qty) && qty < 100)
        {
            QuantityTextBox.Text = (qty + 1).ToString();
        }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (DishComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a dish.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(QuantityTextBox.Text, out var qty) || qty < 1 || qty > 100)
        {
            MessageBox.Show("Please enter a valid quantity (1-100).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SelectedDishId = ((Dish)DishComboBox.SelectedItem).Id;
        Quantity = qty;
        SpecialInstructions = InstructionsTextBox.Text;

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public OrderItem GetOrderItem()
    {
        var dish = _dishes.FirstOrDefault(d => d.Id == SelectedDishId);

        return new OrderItem
        {
            OrderId = OrderId,
            DishId = SelectedDishId ?? 0,
            Quantity = Quantity,
            UnitPrice = dish?.Price ?? 0m,
            SpecialInstructions = SpecialInstructions,
            Status = OrderStatus.New
        };
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
        {
            try { DragMove(); } catch { }
        }
    }
}

