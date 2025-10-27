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
}