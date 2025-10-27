using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestaurantSystem.Core.Models;
using System.Threading;

namespace RestaurantSystem.Data.Context;

/// <summary>
/// Main application DbContext
/// </summary>
public class AppDbContext : DbContext
{
    private readonly IConfiguration? _configuration;

    public DbSet<Table> Tables { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration? configuration = null)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration?.GetConnectionString("DefaultConnection") 
                                  ?? "Data Source=restaurant.db";
            optionsBuilder.UseSqlite(connectionString);
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

}

/// <summary>
/// Interface for entities that need audit fields
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}