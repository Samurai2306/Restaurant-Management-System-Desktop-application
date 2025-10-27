using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.Data.Context;
using RestaurantSystem.Data.Repositories;

namespace RestaurantSystem.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(
      this IServiceCollection services,
 string connectionString)
    {
 // Register DbContext
      services.AddDbContext<AppDbContext>(options =>
     options.UseSqlite(connectionString));

      // Register repositories
        services.AddScoped<ITableRepository, TableRepository>();
      services.AddScoped<IReservationRepository, ReservationRepository>();
      services.AddScoped<IDishRepository, DishRepository>();
      services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
 using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
  await context.InitializeDatabaseAsync();
    }
}