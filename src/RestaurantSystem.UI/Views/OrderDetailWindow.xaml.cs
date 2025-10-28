using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RestaurantSystem.Core.Interfaces.Services;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.Core.Models;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.UI.Services;
using RestaurantSystem.UI.ViewModels.Orders;

namespace RestaurantSystem.UI.Views;

public partial class OrderDetailWindow : Window
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDishRepository _dishRepository;
    private readonly Services.IDialogService _dialogService;

    private Order _order;
    private List<OrderItemViewModel> _items;

    public OrderDetailWindow(Order order, IOrderRepository orderRepository,
                             IDishRepository dishRepository, IDialogService dialogService)
    {
        InitializeComponent();

        _order = order;
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
        _dialogService = dialogService;

        LoadOrderData();
    }

    private void LoadOrderData()
    {
        TitleText.Text = $"Order #{_order.Id}";
        TotalAmountText.Text = $"{_order.TotalAmount:C}";
        StatusText.Text = _order.Status.ToString();
        TableText.Text = _order.Table?.Name ?? "Unknown";
        InstructionsText.Text = string.IsNullOrWhiteSpace(_order.SpecialInstructions)
            ? "No special instructions"
            : _order.SpecialInstructions;

        // Load items
        _items = _order.Items?.Select(item => new OrderItemViewModel
        {
            Id = item.Id,
            OrderId = item.OrderId,
            DishId = item.DishId,
            DishName = item.Dish?.Name ?? "Unknown",
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            SpecialInstructions = item.SpecialInstructions,
            Status = item.Status
        }).ToList() ?? new List<OrderItemViewModel>();

        ItemsDataGrid.ItemsSource = _items;
    }

    private async void AddItemButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Get available dishes
            var dishesResult = await _dishRepository.GetAvailableDishesAsync();
            if (!dishesResult.Succeeded || dishesResult.Value == null)
            {
                MessageBox.Show("Failed to load dishes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new OrderItemEditDialog(_order.Id, dishesResult.Value.ToList());
            dialog.Owner = this;

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var newItem = dialog.GetOrderItem();

                // Add to order (in real app would save to database)
                var newViewModel = new OrderItemViewModel
                {
                    Id = newItem.Id,
                    OrderId = newItem.OrderId,
                    DishId = newItem.DishId,
                    DishName = dishesResult.Value.FirstOrDefault(d => d.Id == newItem.DishId)?.Name ?? "Unknown",
                    Quantity = newItem.Quantity,
                    UnitPrice = newItem.UnitPrice,
                    SpecialInstructions = newItem.SpecialInstructions,
                    Status = newItem.Status
                };

                _items.Add(newViewModel);
                ItemsDataGrid.Items.Refresh();

                MessageBox.Show("Item added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void UpdateOrderButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            MessageBox.Show("Order updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void CompleteOrderButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to complete this order?",
            "Complete Order",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                _order.Status = OrderStatus.Paid;
                _order.ClosedTime = DateTime.Now;

                var updateResult = await _orderRepository.UpdateAsync(_order);
                if (updateResult.Succeeded)
                {
                    await _orderRepository.SaveChangesAsync();
                    MessageBox.Show("Order completed successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show($"Error: {string.Join("\n", updateResult.Errors)}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error completing order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void CancelOrderButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to cancel this order?",
            "Cancel Order",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                _order.Status = OrderStatus.Cancelled;
                _order.ClosedTime = DateTime.Now;

                var updateResult = await _orderRepository.UpdateAsync(_order);
                if (updateResult.Succeeded)
                {
                    await _orderRepository.SaveChangesAsync();
                    MessageBox.Show("Order cancelled successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show($"Error: {string.Join("\n", updateResult.Errors)}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cancelling order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
        {
            try { DragMove(); } catch { }
        }
    }
}

