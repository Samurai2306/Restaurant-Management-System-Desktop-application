using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RestaurantSystem.Data.Context;

/// <summary>
/// Factory for creating DbContext at design time (for EF migrations)
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Use a default connection string for design-time operations
        optionsBuilder.UseSqlite("Data Source=restaurant.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}

