using System.Windows;
using MahApps.Metro.Controls;

namespace RestaurantSystem.UI;

public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Set window properties
        Title = "Restaurant Management System";
        Width = 1280;
        Height = 720;
        WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
    }

    private void NotificationButton_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            "🔔 Notifications\n\nYou have 3 pending reservations and 2 active orders.",
            "Notifications",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            "⚙️ Settings\n\nApplication settings will be implemented here.\n\nFeatures to come:\n- Theme selection (Light/Dark)\n- Database connection settings\n- User preferences",
            "Settings",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            "👤 Account Profile\n\nWelcome to Restaurant Management System!\n\nCurrently logged in as: Admin\nRole: Administrator\nAccount settings will be available here.",
            "Account",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}