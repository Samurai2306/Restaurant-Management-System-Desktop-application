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
    services.AddDataServices(configuration.GetConnectionString("DefaultConnection"));

     // Register ViewModels
    services.AddSingleton<MainWindowViewModel>();
    services.AddTransient<TablesViewModel>();
     services.AddTransient<ReservationsViewModel>();
     services.AddTransient<MenuViewModel>();
    services.AddTransient<OrdersViewModel>();

        // Register Views
        services.AddSingleton<MainWindow>();

        // Register other services
        RegisterServices(services);
    }

    private void RegisterServices(IServiceCollection services)
    {
    // Add services here
 }

    protected override async void OnStartup(StartupEventArgs e)
  {
        await _host.StartAsync();

        // Initialize database
        await _host.Services.InitializeDatabaseAsync();

        // Get the main window from DI and show it
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
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