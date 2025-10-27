using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.UI.Views;

public partial class ReservationEditDialog : Window
{
    public string ClientName { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; } = DateTime.Today.AddDays(1);
    public DateTime StartTime { get; set; } = DateTime.Now.AddHours(1);
    public double DurationHours { get; set; } = 2;
    public int TableId { get; set; } = 1;
    public int GuestsCount { get; set; } = 2;
    public string Comment { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public int? ReservationId { get; set; }
    public List<Table> Tables { get; set; } = new();
    private bool IsEditMode = false;

    public ReservationEditDialog(List<Table> tables)
    {
        InitializeComponent();
        Tables = tables;
        
        Loaded += ReservationEditDialog_Loaded;
    }

    private void ReservationEditDialog_Loaded(object sender, RoutedEventArgs e)
    {
        // Set default date
        DatePicker.SelectedDate = DateTime.Today.AddDays(1);
        
        // Populate time slots
        for (int hour = 9; hour < 23; hour++)
        {
            for (int minute = 0; minute < 60; minute += 30)
            {
                var time = new TimeSpan(hour, minute, 0);
                var item = new ComboBoxItem { Content = time.ToString(@"hh\:mm"), Tag = time };
                TimeComboBox.Items.Add(item);
            }
        }
        
        // Set initial time to next hour
        var currentTime = DateTime.Now.AddHours(1);
        var initialTime = new TimeSpan(currentTime.Hour, (currentTime.Minute / 30) * 30, 0);
        var initialItem = TimeComboBox.Items.OfType<ComboBoxItem>()
            .FirstOrDefault(i => i.Tag is TimeSpan ts && ts == initialTime);
        if (initialItem != null) TimeComboBox.SelectedItem = initialItem;
        
        // Populate tables
        TableComboBox.ItemsSource = Tables;
        TableComboBox.SelectedIndex = 0;
        TableComboBox.SelectionChanged += (s, e) => UpdateTableInfo();
        
        UpdateTableInfo();
    }

    private void UpdateTableInfo()
    {
        if (TableComboBox.SelectedItem is Table selectedTable)
        {
            TableInfoTextBlock.Text = $"{selectedTable.Location} - {selectedTable.SeatsCount} seats";
        }
    }

    public ReservationEditDialog(Reservation reservation, List<Table> tables) : this(tables)
    {
        IsEditMode = true;
        Title = "Edit Reservation";
        TitleTextBlock.Text = "Edit Reservation";
        StatusSection.Visibility = Visibility.Visible;
        
        ReservationId = reservation.Id;
        ClientName = reservation.ClientName;
        ClientPhone = reservation.ClientPhone;
        ReservationDate = reservation.StartTime.Date;
        StartTime = reservation.StartTime;
        DurationHours = (reservation.EndTime - reservation.StartTime).TotalHours;
        TableId = reservation.TableId;
        Comment = reservation.Comment ?? string.Empty;
        Status = reservation.Status.ToString();
        
        // Set fields after loading
        Loaded += (s, e) =>
        {
            ClientNameTextBox.Text = ClientName;
            ClientPhoneTextBox.Text = ClientPhone;
            DatePicker.SelectedDate = ReservationDate;
            
            // Select time
            var time = StartTime.TimeOfDay;
            var timeItem = TimeComboBox.Items.OfType<ComboBoxItem>()
                .FirstOrDefault(i => i.Tag is TimeSpan ts && Math.Abs((ts - time).TotalMinutes) < 30);
            if (timeItem != null) TimeComboBox.SelectedItem = timeItem;
            
            // Select duration
            var durations = new[] { 1.0, 1.5, 2.0, 2.5, 3.0, 3.5, 4.0 };
            var durationIndex = Array.IndexOf(durations, DurationHours);
            if (durationIndex >= 0) DurationComboBox.SelectedIndex = durationIndex;
            
            // Select table
            var tableItem = Tables.FirstOrDefault(t => t.Id == TableId);
            if (tableItem != null) TableComboBox.SelectedItem = tableItem;
            
            // Select guests
            var guestItem = GuestComboBox.Items.OfType<ComboBoxItem>()
                .FirstOrDefault(i => i.Tag?.ToString() == GuestsCount.ToString());
            if (guestItem != null) GuestComboBox.SelectedItem = guestItem;
            
            CommentTextBox.Text = Comment;
            
            // Select status
            var statusItem = StatusComboBox.Items.OfType<ComboBoxItem>()
                .FirstOrDefault(i => i.Tag?.ToString() == Status);
            if (statusItem != null) StatusComboBox.SelectedItem = statusItem;
        };
    }

    private bool ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;
        // Remove spaces and common separators
        var cleaned = Regex.Replace(phone, @"[\s\-\(\)]+", "");
        // Check if it's a valid phone number (7-15 digits, optionally with + prefix)
        return Regex.IsMatch(cleaned, @"^\+?\d{7,15}$");
    }

    private bool ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return true; // Optional field
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(ClientNameTextBox.Text))
        {
            MessageBox.Show("Client name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            ClientNameTextBox.Focus();
            return;
        }

        if (!ValidatePhone(ClientPhoneTextBox.Text))
        {
            MessageBox.Show("Please enter a valid phone number.\nFormat: +1234567890 or 1234567890", 
                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            ClientPhoneTextBox.Focus();
            return;
        }

        if (!ValidateEmail(ClientEmailTextBox.Text))
        {
            MessageBox.Show("Please enter a valid email address or leave it empty.", 
                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            ClientEmailTextBox.Focus();
            return;
        }

        if (DatePicker.SelectedDate == null || DatePicker.SelectedDate < DateTime.Today)
        {
            MessageBox.Show("Please select a valid date (today or later).", 
                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            DatePicker.Focus();
            return;
        }

        if (TimeComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            TimeComboBox.Focus();
            return;
        }

        if (DurationComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a duration.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            DurationComboBox.Focus();
            return;
        }

        if (TableComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a table.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            TableComboBox.Focus();
            return;
        }

        if (GuestComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select the number of guests.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            GuestComboBox.Focus();
            return;
        }

        // Check if guests exceed table capacity
        var selectedTable = TableComboBox.SelectedItem as Table;
        if (GuestComboBox.SelectedItem is ComboBoxItem guestItem)
        {
            var guestCountStr = guestItem.Tag?.ToString();
            if (!string.IsNullOrEmpty(guestCountStr) && int.TryParse(guestCountStr, out var guestCount))
            {
                if (guestCount > selectedTable?.SeatsCount)
                {
                    var result = MessageBox.Show(
                        $"The selected table has {selectedTable.SeatsCount} seats, but you selected {guestCount} guests. Continue anyway?",
                        "Capacity Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No) return;
                }
            }
        }

        // Get values
        ClientName = ClientNameTextBox.Text.Trim();
        ClientPhone = ClientPhoneTextBox.Text.Trim();
        ClientEmail = ClientEmailTextBox.Text.Trim();
        ReservationDate = DatePicker.SelectedDate.Value;
        
        if (TimeComboBox.SelectedItem is ComboBoxItem timeItem && timeItem.Tag is TimeSpan time)
        {
            StartTime = ReservationDate.Add(time);
        }
        
        if (DurationComboBox.SelectedItem is ComboBoxItem durationItem && durationItem.Tag is string durationStr)
        {
            DurationHours = double.Parse(durationStr);
        }
        
        TableId = selectedTable.Id;
        
        if (GuestComboBox.SelectedItem is ComboBoxItem guestItem2 && guestItem2.Tag is string guestsStr)
        {
            GuestsCount = int.Parse(guestsStr);
        }
        
        Comment = CommentTextBox.Text?.Trim() ?? string.Empty;
        
        if (IsEditMode && StatusComboBox.SelectedItem is ComboBoxItem statusItem)
        {
            Status = statusItem.Tag?.ToString() ?? "Pending";
        }
        
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public Reservation GetReservation()
    {
        var startTime = ReservationDate.Date.Add(StartTime.TimeOfDay);
        var endTime = startTime.AddHours(DurationHours);

        var reservation = new Reservation
        {
            ClientName = ClientName,
            ClientPhone = ClientPhone,
            StartTime = startTime,
            EndTime = endTime,
            TableId = TableId,
            Comment = Comment,
            Status = Enum.TryParse<ReservationStatus>(Status, out var status) ? status : ReservationStatus.Pending
        };

        if (ReservationId.HasValue)
        {
            reservation.Id = ReservationId.Value;
        }

        return reservation;
    }
}
