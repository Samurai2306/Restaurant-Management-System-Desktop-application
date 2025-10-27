using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

    builder.HasKey(o => o.Id);

        builder.Property(o => o.CreatedTime)
   .IsRequired();

        builder.Property(o => o.ClosedTime);

        builder.Property(o => o.SpecialInstructions)
            .HasMaxLength(500);

        builder.Property(o => o.Status)
      .IsRequired();

        builder.Property(o => o.WaiterId)
            .HasMaxLength(50);

        builder.Property(o => o.CreatedAt)
         .IsRequired();

        builder.Property(o => o.UpdatedAt)
         .IsRequired();

        // Relationships
        builder.HasOne(o => o.Table)
     .WithMany(t => t.Orders)
     .HasForeignKey(o => o.TableId)
   .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(oi => oi.Order)
     .HasForeignKey(oi => oi.OrderId)
  .OnDelete(DeleteBehavior.Cascade);

      // Indexes
        builder.HasIndex(o => o.CreatedTime);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.WaiterId);
    }
}