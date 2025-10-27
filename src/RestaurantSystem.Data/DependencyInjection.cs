using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.Data.Context;
using RestaurantSystem.Data.Repositories;
using System.Linq;

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
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            // Add seed data only if tables are empty
            if (!context.Tables.Any())
            {
                var tables = new[]
                {
                    new RestaurantSystem.Core.Models.Table { Name = "Table 1", Location = RestaurantSystem.Core.Enums.TableLocation.MainHall, SeatsCount = 4 },
                    new RestaurantSystem.Core.Models.Table { Name = "Table 2", Location = RestaurantSystem.Core.Enums.TableLocation.Window, SeatsCount = 2 },
                    new RestaurantSystem.Core.Models.Table { Name = "Table 3", Location = RestaurantSystem.Core.Enums.TableLocation.Terrace, SeatsCount = 6 }
                };
                context.Tables.AddRange(tables);
                
                var dishes = new[]
                {
                    new RestaurantSystem.Core.Models.Dish { Name = "Caesar Salad", Category = RestaurantSystem.Core.Enums.DishCategory.Salad, Price = 12.99m, CookingTimeMinutes = 15, Description = "Fresh salad", ImagePath = "" },
                    new RestaurantSystem.Core.Models.Dish { Name = "Margherita Pizza", Category = RestaurantSystem.Core.Enums.DishCategory.MainCourse, Price = 15.99m, CookingTimeMinutes = 20, Description = "Classic pizza", ImagePath = "" },
                    new RestaurantSystem.Core.Models.Dish { Name = "Tiramisu", Category = RestaurantSystem.Core.Enums.DishCategory.Dessert, Price = 8.99m, CookingTimeMinutes = 10, Description = "Italian dessert", ImagePath = "" }
                };
                context.Dishes.AddRange(dishes);
                
                await context.SaveChangesAsync(default);
            }
        }
        catch
        {
            // Ignore initialization errors - database might already exist
        }
    }
}