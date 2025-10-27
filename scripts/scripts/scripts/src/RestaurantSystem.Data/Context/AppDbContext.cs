using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.Data.Context;

/// <summary>
/// Main application DbContext
/// </summary>
public class AppDbContext : DbContext
{
  private readonly IConfiguration _configuration;

    public DbSet<Table> Tables { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<Order> Orders { get; set; }
public DbSet<OrderItem> OrderItems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
    optionsBuilder.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
        }
    }

 protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Add any additional configuration here
        modelBuilder.Entity<Table>()
            .HasQueryFilter(t => !t.IsDeleted);

        modelBuilder.Entity<Dish>()
            .HasQueryFilter(d => !d.IsDeleted);

        modelBuilder.Entity<Order>()
            .HasQueryFilter(o => !o.IsDeleted);

        modelBuilder.Entity<Reservation>()
       .HasQueryFilter(r => !r.IsDeleted);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
     UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
   var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
     var entity = (BaseEntity)entry.Entity;

     if (entry.State == EntityState.Added)
{
       entity.CreatedAt = now;
          }
            
            entity.UpdatedAt = now;
  }
    }

    /// <summary>
    /// Initialize database with seed data
  /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        // Ensure database is created
        await Database.EnsureCreatedAsync();

        // Add seed data if the database was just created
        if (!Tables.Any())
  {
            // Add sample tables
     var tables = new[]
      {
   new Table { Name = "Table 1", Location = Core.Enums.TableLocation.MainHall, SeatsCount = 4 },
              new Table { Name = "Table 2", Location = Core.Enums.TableLocation.Window, SeatsCount = 2 },
             new Table { Name = "Table 3", Location = Core.Enums.TableLocation.Terrace, SeatsCount = 6 }
  };
        Tables.AddRange(tables);
         
            // Add sample dishes
      var dishes = new[]
      {
  new Dish { Name = "Caesar Salad", Category = Core.Enums.DishCategory.Salad, Price = 12.99m, CookingTimeMinutes = 15 },
     new Dish { Name = "Margherita Pizza", Category = Core.Enums.DishCategory.MainCourse, Price = 15.99m, CookingTimeMinutes = 20 },
       new Dish { Name = "Tiramisu", Category = Core.Enums.DishCategory.Dessert, Price = 8.99m, CookingTimeMinutes = 10 }
            };
            Dishes.AddRange(dishes);

      await SaveChangesAsync();
   }
    }
}

/// <summary>
/// Interface for entities that need audit fields
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}