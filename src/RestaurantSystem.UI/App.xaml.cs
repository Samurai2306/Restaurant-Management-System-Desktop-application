using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestaurantSystem.UI.ViewModels;
using RestaurantSystem.UI.ViewModels.Tables;
using RestaurantSystem.UI.ViewModels.Reservations;
using RestaurantSystem.UI.ViewModels.Menu;
using RestaurantSystem.UI.ViewModels.Orders;
using System.IO;
using Microsoft.Extensions.Configuration;
using RestaurantSystem.Data;

namespace RestaurantSystem.UI;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
  {
        _host = Host.CreateDefaultBuilder()
      .ConfigureAppConfiguration((context, config) =>
     {
 config.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
   .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
            })
     .ConfigureServices((context, services) =>
      {
          ConfigureServices(services, context.Configuration);
            })
            .Build();
    }

    private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register database
        services.AddDataServices(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=restaurant.db");

        // Register UI Services
        services.AddSingleton<Services.IDialogService, Services.DialogService>();
        services.AddSingleton<Services.INavigationService, Services.NavigationService>();
        
        // Register MainWindow
        services.AddSingleton<MainWindow>();
        
        // Register ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<TablesViewModel>();
        services.AddTransient<ViewModels.Tables.TableViewModel>();
        services.AddTransient<ReservationsViewModel>();
        services.AddTransient<ViewModels.Reservations.ReservationViewModel>();
        services.AddTransient<MenuViewModel>();
        services.AddTransient<ViewModels.Menu.DishViewModel>();
        services.AddTransient<OrdersViewModel>();
        services.AddTransient<ViewModels.Orders.OrderViewModel>();
        services.AddTransient<ViewModels.Orders.OrderItemViewModel>();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            await _host.StartAsync();

            // Initialize database
            await _host.Services.InitializeDatabaseAsync();

            // Get the main window and ViewModel from DI
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            var mainWindowViewModel = _host.Services.GetRequiredService<MainWindowViewModel>();
            
            // Set DataContext
            mainWindow.DataContext = mainWindowViewModel;
            
            // Navigate to Tables view by default
            mainWindowViewModel.NavigateTablesCommand.Execute(null);
            
            // Show the main window
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting application: {ex.Message}\n\n{ex.StackTrace}", 
                "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
   {
  await _host.StopAsync(TimeSpan.FromSeconds(5));
        }

        base.OnExit(e);
    }
}