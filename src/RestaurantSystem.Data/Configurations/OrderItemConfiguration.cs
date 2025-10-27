using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
      builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

  builder.Property(oi => oi.Quantity)
     .IsRequired();

  builder.Property(oi => oi.UnitPrice)
      .IsRequired()
  .HasPrecision(10, 2);

    builder.Property(oi => oi.SpecialInstructions)
 .HasMaxLength(500);

        builder.Property(oi => oi.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(oi => oi.CreatedAt)
            .IsRequired();

        builder.Property(oi => oi.UpdatedAt)
            .IsRequired();

        builder.Property(oi => oi.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Ignore calculated property
        builder.Ignore(oi => oi.TotalPrice);

        // Relationships
        builder.HasOne(oi => oi.Order)
       .WithMany(o => o.Items)
       .HasForeignKey(oi => oi.OrderId)
     .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oi => oi.Dish)
     .WithMany(d => d.OrderItems)
   .HasForeignKey(oi => oi.DishId)
    .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(oi => oi.Status);
        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.DishId);
        builder.HasIndex(oi => oi.IsDeleted);
        builder.HasIndex(oi => new { oi.OrderId, oi.DishId });
    }
}