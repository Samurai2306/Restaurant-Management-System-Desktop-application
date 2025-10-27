using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.Data.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
 builder.ToTable("Tables");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
         .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Location)
          .IsRequired();

        builder.Property(t => t.SeatsCount)
      .IsRequired();

        builder.Property(t => t.IsActive)
    .IsRequired()
        .HasDefaultValue(true);

        builder.Property(t => t.CreatedAt)
       .IsRequired();

    builder.Property(t => t.UpdatedAt)
     .IsRequired();

     // Relationships
        builder.HasMany(t => t.Reservations)
      .WithOne(r => r.Table)
.HasForeignKey(r => r.TableId)
     .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Orders)
            .WithOne(o => o.Table)
            .HasForeignKey(o => o.TableId)
    .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(t => t.Name).IsUnique();
        builder.HasIndex(t => t.Location);
    }
}