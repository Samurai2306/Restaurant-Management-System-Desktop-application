using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace RestaurantSystem.UI;

public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Set window properties
        Title = "Система Управления Рестораном";
        Width = 1280;
        Height = 720;
        WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        
        // Handle window closing to prevent accidental shutdown
        Closing += MainWindow_Closing;
    }
    
    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        // Prevent app shutdown if this is the main window
        if (Application.Current.MainWindow == this)
        {
            // Do not prevent closing, just log
            Console.WriteLine("Main window is closing");
        }
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
        try
        {
            var settingsManager = new Services.SettingsManager();
            var settingsWindow = new Views.SettingsWindow(settingsManager);
            settingsWindow.Owner = this;
            
            var result = settingsWindow.ShowDialog();
            if (result == true)
            {
                System.Windows.MessageBox.Show(
                    "Settings have been saved. Some changes may require restarting the application.",
                    "Settings Saved",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Error opening settings: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var authService = ((App)Application.Current).ServiceProvider.GetRequiredService<RestaurantSystem.Core.Interfaces.Services.IAuthenticationService>();
            
            var dialog = new Views.ProfileDialog(authService);
            dialog.Owner = this;
            dialog.ShowDialog();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Error opening profile: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to logout?",
            "Logout Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            // Close main window and show login window again
            var authService = ((App)Application.Current).ServiceProvider.GetRequiredService<RestaurantSystem.Core.Interfaces.Services.IAuthenticationService>();
            authService.Logout();
            
            Close();
            
            // Show login window
            var loginWindow = new Views.LoginWindow(authService);
            var loginResult = loginWindow.ShowDialog();
            
            if (loginResult == true && loginWindow.LoggedInUser != null)
            {
                // User logged in again, open main window
                Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}