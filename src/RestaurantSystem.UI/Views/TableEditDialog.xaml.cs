using System;
using System.Windows;
using System.Windows.Controls;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.UI.Views;

public partial class TableEditDialog : Window
{
    public string Title { get; set; } = "Add Table";
    public string TableName { get; set; } = string.Empty;
    public TableLocation Location { get; set; } = TableLocation.MainHall;
    public int SeatsCount { get; set; } = 2;
    private bool _isTableActive = true;
    public bool IsTableActive
    {
        get => _isTableActive;
        set => _isTableActive = value;
    }
    public int? TableId { get; set; }

    public TableEditDialog()
    {
        try
        {
            InitializeComponent();
            DataContext = this;

            // Set default selection after the component is initialized
            Loaded += (s, e) =>
            {
                if (LocationComboBox.Items.Count > 0)
                {
                    LocationComboBox.SelectedIndex = 0;
                }
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing dialog: {ex.Message}\n\n{ex.StackTrace}",
                "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public TableEditDialog(Table table) : this()
    {
        try
        {
            Title = "Edit Table";
            TableId = table.Id;
            TableName = table.Name;
            Location = table.Location;
            SeatsCount = table.SeatsCount;
            IsTableActive = table.IsActive;

            // Set location combo box after loaded
            Loaded += (s, e) =>
            {
                switch (table.Location)
                {
                    case TableLocation.MainHall:
                        LocationComboBox.SelectedIndex = 0;
                        break;
                    case TableLocation.Terrace:
                        LocationComboBox.SelectedIndex = 1;
                        break;
                    case TableLocation.Window:
                        LocationComboBox.SelectedIndex = 2;
                        break;
                    case TableLocation.VipRoom:
                        LocationComboBox.SelectedIndex = 3;
                        break;
                    case TableLocation.Bar:
                        LocationComboBox.SelectedIndex = 4;
                        break;
                }
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading table data: {ex.Message}\n\n{ex.StackTrace}",
                "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(TableName))
        {
            MessageBox.Show("Please enter a table name.", "Validation Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            NameTextBox.Focus();
            return;
        }

        if (SeatsCount < 1 || SeatsCount > 20)
        {
            MessageBox.Show("Seats count must be between 1 and 20.", "Validation Error",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            SeatsTextBox.Focus();
            return;
        }

        // Get selected location
        var selectedItem = LocationComboBox.SelectedItem as ComboBoxItem;
        if (selectedItem != null)
        {
            Location = selectedItem.Tag?.ToString() switch
            {
                "MainHall" => TableLocation.MainHall,
                "Terrace" => TableLocation.Terrace,
                "Window" => TableLocation.Window,
                "VIPRoom" => TableLocation.VipRoom,
                "Bar" => TableLocation.Bar,
                _ => TableLocation.MainHall
            };
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public Table GetTable()
    {
        var table = new Table
        {
            Name = TableName,
            Location = Location,
            SeatsCount = SeatsCount,
            IsActive = IsTableActive
        };

        if (TableId.HasValue)
        {
            table.Id = TableId.Value;
        }

        return table;
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
        {
            try { DragMove(); } catch { }
        }
    }
}

