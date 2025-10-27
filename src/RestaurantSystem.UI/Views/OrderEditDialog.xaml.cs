using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.UI.Views;

public partial class OrderEditDialog : Window
{
    public int TableId { get; set; } = 1;
    public string WaiterId { get; set; } = "WAITER01";
    public string SpecialInstructions { get; set; } = string.Empty;
    public List<Table> Tables { get; set; } = new();

    public OrderEditDialog(List<Table> tables)
    {
        InitializeComponent();
        Tables = tables;
        
        // Populate tables
        TableComboBox.ItemsSource = tables;
        if (tables.Any()) TableComboBox.SelectedIndex = 0;
    }

    public OrderEditDialog(Order order, List<Table> tables) : this(tables)
    {
        Title = "Edit Order";
        TableId = order.TableId;
        WaiterId = order.WaiterId ?? "WAITER01";
        SpecialInstructions = order.SpecialInstructions ?? string.Empty;
        
        // Set fields
        var tableItem = TableComboBox.Items.OfType<Table>().FirstOrDefault(t => t.Id == TableId);
        if (tableItem != null) TableComboBox.SelectedItem = tableItem;
        
        WaiterTextBox.Text = WaiterId;
        InstructionsTextBox.Text = SpecialInstructions;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (TableComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a table.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        TableId = ((Table)TableComboBox.SelectedItem).Id;
        WaiterId = WaiterTextBox.Text;
        SpecialInstructions = InstructionsTextBox.Text;
        
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public Order GetOrder()
    {
        return new Order
        {
            TableId = TableId,
            WaiterId = WaiterId,
            SpecialInstructions = SpecialInstructions,
            CreatedTime = DateTime.UtcNow
        };
    }
}
