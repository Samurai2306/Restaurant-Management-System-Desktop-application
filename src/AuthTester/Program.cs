using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RestaurantSystem.Data;
using RestaurantSystem.Core.Interfaces.Services;

Console.WriteLine("AuthTester starting...");

var host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var connection = context.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=restaurant.db";

        // Register data services
        services.AddDataServices(connection);

        // Register authentication service as scoped (same as App.xaml.cs change)
        services.AddScoped<RestaurantSystem.Core.Interfaces.Services.IAuthenticationService, RestaurantSystem.UI.Services.AuthenticationService>();
    })
    .Build();

await host.StartAsync();

// Initialize database (seeding)
await host.Services.InitializeDatabaseAsync();

var auth = host.Services.GetRequiredService<IAuthenticationService>();

Console.WriteLine("Calling LoginAsync with admin/admin123");
var user = await auth.LoginAsync("admin", "admin123");
Console.WriteLine(user == null ? "Login failed" : $"Login succeeded: {user.Username} (Role: {user.Role})");

Console.WriteLine("Calling LoginAsync with waiter/waiter123");
var user2 = await auth.LoginAsync("waiter", "waiter123");
Console.WriteLine(user2 == null ? "Login failed" : $"Login succeeded: {user2.Username} (Role: {user2.Role})");

// Try wrong password
Console.WriteLine("Calling LoginAsync with admin/wrongpass");
var user3 = await auth.LoginAsync("admin", "wrongpass");
Console.WriteLine(user3 == null ? "Login failed (as expected)" : $"Unexpected success: {user3.Username}");

await host.StopAsync();
Console.WriteLine("AuthTester finished");
