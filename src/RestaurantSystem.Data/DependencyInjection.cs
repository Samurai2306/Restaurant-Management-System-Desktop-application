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
      services.AddScoped<IUserRepository, UserRepository>();

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
                // Создаём столы
                var tables = new[]
                {
                    new RestaurantSystem.Core.Models.Table { Name = "Стол 1", Location = RestaurantSystem.Core.Enums.TableLocation.MainHall, SeatsCount = 4 },
                    new RestaurantSystem.Core.Models.Table { Name = "Стол 2", Location = RestaurantSystem.Core.Enums.TableLocation.MainHall, SeatsCount = 4 },
                    new RestaurantSystem.Core.Models.Table { Name = "Стол 3", Location = RestaurantSystem.Core.Enums.TableLocation.Window, SeatsCount = 2 },
                    new RestaurantSystem.Core.Models.Table { Name = "Стол 4", Location = RestaurantSystem.Core.Enums.TableLocation.Window, SeatsCount = 2 },
                    new RestaurantSystem.Core.Models.Table { Name = "Стол 5", Location = RestaurantSystem.Core.Enums.TableLocation.Terrace, SeatsCount = 6 },
                    new RestaurantSystem.Core.Models.Table { Name = "Стол 6", Location = RestaurantSystem.Core.Enums.TableLocation.Terrace, SeatsCount = 6 },
                    new RestaurantSystem.Core.Models.Table { Name = "Стол 7", Location = RestaurantSystem.Core.Enums.TableLocation.MainHall, SeatsCount = 8 },
                    new RestaurantSystem.Core.Models.Table { Name = "Стол 8", Location = RestaurantSystem.Core.Enums.TableLocation.Bar, SeatsCount = 4 },
                    new RestaurantSystem.Core.Models.Table { Name = "VIP 1", Location = RestaurantSystem.Core.Enums.TableLocation.MainHall, SeatsCount = 6 },
                    new RestaurantSystem.Core.Models.Table { Name = "VIP 2", Location = RestaurantSystem.Core.Enums.TableLocation.MainHall, SeatsCount = 10 }
                };
                context.Tables.AddRange(tables);
                
                // Создаём блюда
                var dishes = new[]
                {
                    // Закуски
                    new RestaurantSystem.Core.Models.Dish { Name = "Оливье", Category = RestaurantSystem.Core.Enums.DishCategory.Appetizer, Price = 450, CookingTimeMinutes = 20, Description = "Классический салат с колбасой, картофелем и овощами", ImagePath = "", Tags = "салат, традиционный", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Борщ", Category = RestaurantSystem.Core.Enums.DishCategory.Soup, Price = 380, CookingTimeMinutes = 30, Description = "Традиционный украинский суп со свёклой и сметаной", ImagePath = "", Tags = "суп, традиционный", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Окрошка", Category = RestaurantSystem.Core.Enums.DishCategory.Soup, Price = 320, CookingTimeMinutes = 15, Description = "Холодный суп на квасе с овощами и варёным яйцом", ImagePath = "", Tags = "суп, холодный", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    
                    // Салаты
                    new RestaurantSystem.Core.Models.Dish { Name = "Цезарь с курицей", Category = RestaurantSystem.Core.Enums.DishCategory.Salad, Price = 520, CookingTimeMinutes = 15, Description = "Салат романо с куриным филе, сыром пармезан и соусом цезарь", ImagePath = "", Tags = "салат, курица", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Греческий салат", Category = RestaurantSystem.Core.Enums.DishCategory.Salad, Price = 480, CookingTimeMinutes = 12, Description = "Свежие овощи с сыром фета и оливковым маслом", ImagePath = "", Tags = "салат, овощи", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Овощной салат", Category = RestaurantSystem.Core.Enums.DishCategory.Salad, Price = 360, CookingTimeMinutes = 10, Description = "Свежие сезонные овощи с зеленью", ImagePath = "", Tags = "салат, вегетарианский", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    
                    // Основные блюда
                    new RestaurantSystem.Core.Models.Dish { Name = "Стейк из говядины", Category = RestaurantSystem.Core.Enums.DishCategory.MainCourse, Price = 1850, CookingTimeMinutes = 25, Description = "Премиальный стейк рибай с гарниром и соусом", ImagePath = "", Tags = "мясо, премиум", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Голубцы", Category = RestaurantSystem.Core.Enums.DishCategory.MainCourse, Price = 580, CookingTimeMinutes = 35, Description = "Капустные листья с фаршем в томатном соусе", ImagePath = "", Tags = "традиционный, мясо", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Плов узбекский", Category = RestaurantSystem.Core.Enums.DishCategory.MainCourse, Price = 680, CookingTimeMinutes = 40, Description = "Рис с бараниной и овощами, приготовленный в казане", ImagePath = "", Tags = "мясо, рис", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Котлеты по-киевски", Category = RestaurantSystem.Core.Enums.DishCategory.MainCourse, Price = 720, CookingTimeMinutes = 30, Description = "Куриное филе с маслом и зеленью в панировке", ImagePath = "", Tags = "курица, традиционный", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Лосось на пару", Category = RestaurantSystem.Core.Enums.DishCategory.MainCourse, Price = 980, CookingTimeMinutes = 20, Description = "Филе лосося с овощами и соусом", ImagePath = "", Tags = "рыба, диетический", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Пицца Маргарита", Category = RestaurantSystem.Core.Enums.DishCategory.MainCourse, Price = 650, CookingTimeMinutes = 15, Description = "Тонкое тесто с томатами, моцареллой и базиликом", ImagePath = "", Tags = "пицца, итальянская", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Шашлык из свинины", Category = RestaurantSystem.Core.Enums.DishCategory.MainCourse, Price = 890, CookingTimeMinutes = 30, Description = "Маринованная свинина на углях с гарниром", ImagePath = "", Tags = "мясо, гриль", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    
                    // Гарниры
                    new RestaurantSystem.Core.Models.Dish { Name = "Картофель фри", Category = RestaurantSystem.Core.Enums.DishCategory.SideDish, Price = 280, CookingTimeMinutes = 12, Description = "Хрустящий картофель с соусом", ImagePath = "", Tags = "картофель, гарнир", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Макароны с сыром", Category = RestaurantSystem.Core.Enums.DishCategory.SideDish, Price = 320, CookingTimeMinutes = 15, Description = "Паста с трюфельным маслом", ImagePath = "", Tags = "макароны, сыр", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Гречка с грибами", Category = RestaurantSystem.Core.Enums.DishCategory.SideDish, Price = 380, CookingTimeMinutes = 20, Description = "Гречневая каша с обжаренными грибами", ImagePath = "", Tags = "гречка, грибы, вегетарианский", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    
                    // Десерты
                    new RestaurantSystem.Core.Models.Dish { Name = "Тирамису", Category = RestaurantSystem.Core.Enums.DishCategory.Dessert, Price = 580, CookingTimeMinutes = 15, Description = "Классический итальянский десерт", ImagePath = "", Tags = "десерт, итальянский", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Чизкейк", Category = RestaurantSystem.Core.Enums.DishCategory.Dessert, Price = 520, CookingTimeMinutes = 10, Description = "Нежный творожный торт с ягодами", ImagePath = "", Tags = "десерт, творог", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Морковный торт", Category = RestaurantSystem.Core.Enums.DishCategory.Dessert, Price = 480, CookingTimeMinutes = 12, Description = "Традиционный торт с морковью и кремом", ImagePath = "", Tags = "десерт, традиционный", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Шоколадный фондан", Category = RestaurantSystem.Core.Enums.DishCategory.Dessert, Price = 650, CookingTimeMinutes = 12, Description = "Шоколадный кекс с жидкой начинкой и мороженым", ImagePath = "", Tags = "десерт, шоколад", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    
                    // Напитки
                    new RestaurantSystem.Core.Models.Dish { Name = "Кока-кола", Category = RestaurantSystem.Core.Enums.DishCategory.Beverage, Price = 180, CookingTimeMinutes = 0, Description = "Газированный безалкогольный напиток 0.5л", ImagePath = "", Tags = "напиток, газировка", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Лимонад", Category = RestaurantSystem.Core.Enums.DishCategory.Beverage, Price = 220, CookingTimeMinutes = 0, Description = "Домашний лимонад из лайма и мяты", ImagePath = "", Tags = "напиток, освежающий", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Чай зелёный", Category = RestaurantSystem.Core.Enums.DishCategory.Beverage, Price = 250, CookingTimeMinutes = 5, Description = "Зелёный чай с жасмином", ImagePath = "", Tags = "напиток, горячий", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Кофе эспрессо", Category = RestaurantSystem.Core.Enums.DishCategory.Beverage, Price = 280, CookingTimeMinutes = 5, Description = "Крепкий эспрессо", ImagePath = "", Tags = "напиток, кофе", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    
                    // Алкоголь
                    new RestaurantSystem.Core.Models.Dish { Name = "Водка", Category = RestaurantSystem.Core.Enums.DishCategory.Alcohol, Price = 450, CookingTimeMinutes = 0, Description = "Премиальная водка 50мл", ImagePath = "", Tags = "алкоголь, крепкий", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Вино красное", Category = RestaurantSystem.Core.Enums.DishCategory.Alcohol, Price = 890, CookingTimeMinutes = 0, Description = "Бордо 200мл", ImagePath = "", Tags = "алкоголь, вино", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Вино белое", Category = RestaurantSystem.Core.Enums.DishCategory.Alcohol, Price = 850, CookingTimeMinutes = 0, Description = "Шардоне 200мл", ImagePath = "", Tags = "алкоголь, вино", Allergens = RestaurantSystem.Core.Enums.Allergen.None },
                    new RestaurantSystem.Core.Models.Dish { Name = "Пиво крафтовое", Category = RestaurantSystem.Core.Enums.DishCategory.Alcohol, Price = 420, CookingTimeMinutes = 0, Description = "Крафтовое пиво 0.5л", ImagePath = "", Tags = "алкоголь, пиво", Allergens = RestaurantSystem.Core.Enums.Allergen.None }
                };
                context.Dishes.AddRange(dishes);

                await context.SaveChangesAsync(default);
                Console.WriteLine("Database seeded with Russian test data");
            }

            // Seed default admin user separately
            if (!context.Users.Any())
            {
                Console.WriteLine("Creating default users...");
                
                // Create admin user
                var adminPassword = "admin123";
                var adminHash = HashPassword(adminPassword);
                Console.WriteLine($"Admin user hash created: {adminHash}");
                
                var adminUser = new RestaurantSystem.Core.Models.User
                {
                    Username = "admin",
                    PasswordHash = adminHash,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@restaurant.com",
                    Phone = "1234567890",
                    Role = RestaurantSystem.Core.Enums.UserRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Users.Add(adminUser);

                // Seed a waiter user
                var waiterPassword = "waiter123";
                var waiterHash = HashPassword(waiterPassword);
                Console.WriteLine($"Waiter user hash created: {waiterHash}");
                
                var waiterUser = new RestaurantSystem.Core.Models.User
                {
                    Username = "waiter",
                    PasswordHash = waiterHash,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "waiter@restaurant.com",
                    Phone = "9876543210",
                    Role = RestaurantSystem.Core.Enums.UserRole.Waiter,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Users.Add(waiterUser);

                await context.SaveChangesAsync(default);
                Console.WriteLine("Default users created successfully");
            }
            else
            {
                Console.WriteLine("Users already exist in database");
            }
        }
        catch (Exception ex)
        {
            // Ignore initialization errors - database might already exist
            Console.WriteLine($"Database initialization error: {ex.Message}");
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        var hash = Convert.ToBase64String(hashedBytes);
        Console.WriteLine($"Hashing password 'admin123': {hash}");
        return hash;
    }
}