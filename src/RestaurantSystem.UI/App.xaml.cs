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
using RestaurantSystem.Core.Interfaces.Repositories;

namespace RestaurantSystem.UI;

public partial class App : Application
{
    private readonly IHost _host;

    public IServiceProvider ServiceProvider => _host.Services;

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
        // AuthenticationService depends on IUserRepository and AppDbContext (scoped services).
        // Register AuthenticationService as scoped so it receives scoped repository/DbContext correctly.
        services.AddScoped<RestaurantSystem.Core.Interfaces.Services.IAuthenticationService, Services.AuthenticationService>();

        // Register Windows
        services.AddSingleton<MainWindow>();
        services.AddTransient<Views.LoginWindow>();

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
        services.AddTransient<AnalyticsViewModel>();
    }

    private MainWindowViewModel CreateMainWindowViewModel(IServiceProvider services)
    {
        return new MainWindowViewModel(
            services.GetRequiredService<TablesViewModel>(),
            services.GetRequiredService<ReservationsViewModel>(),
            services.GetRequiredService<MenuViewModel>(),
            services.GetRequiredService<OrdersViewModel>(),
            services.GetRequiredService<AnalyticsViewModel>());
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            await _host.StartAsync();

            // Initialize database
            await _host.Services.InitializeDatabaseAsync();

            // Show login window first
            var authService = _host.Services.GetRequiredService<RestaurantSystem.Core.Interfaces.Services.IAuthenticationService>();
            var loginWindow = new Views.LoginWindow(authService);

            var result = loginWindow.ShowDialog();

            if (result == true && loginWindow.LoggedInUser != null)
            {
                // User logged in successfully, show main window
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                var mainWindowViewModel = CreateMainWindowViewModel(_host.Services);

                // Set DataContext
                mainWindow.DataContext = mainWindowViewModel;

                // Navigate to Tables view by default
                mainWindowViewModel.NavigateTablesCommand.Execute(null);

                // Set as main window BEFORE showing
                MainWindow = mainWindow;

                // Show the main window
                mainWindow.Show();
            }
            else
            {
                // User cancelled login or failed
                Shutdown();
            }
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