using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
 builder.ToTable("Reservations");

     builder.HasKey(r => r.Id);

        builder.Property(r => r.ClientName)
          .IsRequired()
  .HasMaxLength(100);

        builder.Property(r => r.ClientPhone)
     .IsRequired()
        .HasMaxLength(20);

        builder.Property(r => r.StartTime)
      .IsRequired();

      builder.Property(r => r.EndTime)
       .IsRequired();

   builder.Property(r => r.Comment)
         .HasMaxLength(500);

     builder.Property(r => r.Status)
    .IsRequired();

     builder.Property(r => r.CreatedAt)
  .IsRequired();

        builder.Property(r => r.UpdatedAt)
      .IsRequired();

        // Relationships
        builder.HasOne(r => r.Table)
   .WithMany(t => t.Reservations)
  .HasForeignKey(r => r.TableId)
        .OnDelete(DeleteBehavior.Restrict);

     // Indexes
   builder.HasIndex(r => r.ClientPhone);
        builder.HasIndex(r => r.StartTime);
        builder.HasIndex(r => r.Status);
    }
}