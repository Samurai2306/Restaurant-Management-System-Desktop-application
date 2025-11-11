using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.UI.Views;

public partial class OrderEditDialog : Window
{
    public int TableId { get; set; } = 1;
    public int? WaiterId { get; set; }
    public string Status { get; set; } = "New";
    public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    public DateTime? ClosedDateTime { get; set; }
    public List<User> Waiters { get; set; } = new();
    public string SpecialInstructions { get; set; } = string.Empty;
    public List<Table> Tables { get; set; } = new();

    public OrderEditDialog(List<Table> tables)
    {
        InitializeComponent();
        Tables = tables;

        // Populate tables
        TableComboBox.ItemsSource = tables;
        if (tables.Any()) TableComboBox.SelectedIndex = 0;

        // TODO: Загрузить список официантов из БД/сервиса
        // Пример: Waiters = UserService.GetWaiters();
        // Пока что добавим тестовые данные:
        Waiters = new List<User> {
            new User { Id = 1, FirstName = "Иван", LastName = "Иванов" },
            new User { Id = 2, FirstName = "Пётр", LastName = "Петров" },
            new User { Id = 3, FirstName = "Сидор", LastName = "Сидоров" }
        };
        WaiterComboBox.ItemsSource = Waiters;
        WaiterComboBox.DisplayMemberPath = "FullName";
        WaiterComboBox.SelectedValuePath = "Id";
        WaiterComboBox.SelectedIndex = 0;

        // Статус заказа
        StatusComboBox.SelectedIndex = 0;

        // Время создания по умолчанию
        CreatedDatePicker.SelectedDate = DateTime.Now.Date;
        CreatedTimeTextBox.Text = DateTime.Now.ToString("HH:mm");
    }

    public OrderEditDialog(Order order, List<Table> tables) : this(tables)
    {
        Title = "Edit Order";
        TableId = order.TableId;
        WaiterId = order.WaiterId;
        SpecialInstructions = order.SpecialInstructions ?? string.Empty;
        Status = order.Status.ToString();
        CreatedDateTime = order.CreatedTime;
        ClosedDateTime = order.ClosedTime;

        // Set fields
        var tableItem = TableComboBox.Items.OfType<Table>().FirstOrDefault(t => t.Id == TableId);
        if (tableItem != null) TableComboBox.SelectedItem = tableItem;

        if (WaiterId.HasValue)
        {
            var waiterItem = Waiters.FirstOrDefault(w => w.Id == WaiterId.Value);
            if (waiterItem != null) WaiterComboBox.SelectedItem = waiterItem;
        }

        InstructionsTextBox.Text = SpecialInstructions;
        // Статус
        var statusItem = StatusComboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(i => i.Tag?.ToString() == Status);
        if (statusItem != null) StatusComboBox.SelectedItem = statusItem;

        // Время создания
        CreatedDatePicker.SelectedDate = CreatedDateTime.Date;
        CreatedTimeTextBox.Text = CreatedDateTime.ToString("HH:mm");

        // Время закрытия
        if (ClosedDateTime.HasValue)
        {
            ClosedDatePicker.SelectedDate = ClosedDateTime.Value.Date;
            ClosedTimeTextBox.Text = ClosedDateTime.Value.ToString("HH:mm");
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (TableComboBox.SelectedItem == null)
        {
            MessageBox.Show("Выберите стол.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        TableId = ((Table)TableComboBox.SelectedItem).Id;

        if (WaiterComboBox.SelectedItem is User waiter)
            WaiterId = waiter.Id;
        else
            WaiterId = null;

        SpecialInstructions = InstructionsTextBox.Text;

        // Статус
        if (StatusComboBox.SelectedItem is ComboBoxItem statusItem)
            Status = statusItem.Tag?.ToString() ?? "New";
        else
            Status = "New";

        // Время создания
        if (CreatedDatePicker.SelectedDate.HasValue && TimeSpan.TryParse(CreatedTimeTextBox.Text, out var ctime))
            CreatedDateTime = CreatedDatePicker.SelectedDate.Value.Date + ctime;
        else
            CreatedDateTime = DateTime.Now;

        // Время закрытия
        if (ClosedDatePicker.SelectedDate.HasValue && TimeSpan.TryParse(ClosedTimeTextBox.Text, out var cltime))
            ClosedDateTime = ClosedDatePicker.SelectedDate.Value.Date + cltime;
        else
            ClosedDateTime = null;

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
            CreatedTime = CreatedDateTime,
            ClosedTime = ClosedDateTime,
            Status = Enum.TryParse<RestaurantSystem.Core.Enums.OrderStatus>(Status, out var st) ? st : RestaurantSystem.Core.Enums.OrderStatus.New
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
