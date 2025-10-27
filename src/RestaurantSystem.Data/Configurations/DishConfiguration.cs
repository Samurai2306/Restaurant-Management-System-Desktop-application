using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.Data.Configurations;

public class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
 builder.ToTable("Dishes");

        builder.HasKey(d => d.Id);

  builder.Property(d => d.Name)
    .IsRequired()
       .HasMaxLength(100);

        builder.Property(d => d.Description)
       .HasMaxLength(500);

        builder.Property(d => d.Price)
    .IsRequired()
    .HasPrecision(10, 2);

        builder.Property(d => d.Category)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(d => d.CookingTimeMinutes)
            .IsRequired();

        builder.Property(d => d.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.ImagePath)
            .HasMaxLength(255);

        builder.Property(d => d.Tags)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(d => d.Allergens)
            .HasConversion<string>();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired();

        builder.Property(d => d.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasMany(d => d.OrderItems)
        .WithOne(oi => oi.Dish)
 .HasForeignKey(oi => oi.DishId)
        .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(d => d.Name);
        builder.HasIndex(d => d.Category);
        builder.HasIndex(d => d.IsAvailable);
        builder.HasIndex(d => d.IsDeleted);
        builder.HasIndex(d => new { d.Category, d.IsAvailable });
    }
}