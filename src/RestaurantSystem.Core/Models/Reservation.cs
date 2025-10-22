using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.Core.Models;

public class Reservation : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string ClientName { get; set; }

    [Required]
    [Phone]
    [StringLength(20)]
    public string ClientPhone { get; set; }

    [Required]
 public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

  [Required]
    public int TableId { get; set; }

    [StringLength(500)]
    public string Comment { get; set; }

    [Required]
    public ReservationStatus Status { get; set; }

    // Navigation properties
    public virtual Table Table { get; set; }

    // Validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
{
        if (EndTime <= StartTime)
        {
   yield return new ValidationResult(
  "End time must be after start time",
            new[] { nameof(EndTime) });
        }

        var duration = EndTime - StartTime;
        if (duration.TotalMinutes < 30)
        {
         yield return new ValidationResult(
          "Reservation must be at least 30 minutes long",
          new[] { nameof(EndTime) });
        }

        if (duration.TotalHours > 4)
 {
       yield return new ValidationResult(
           "Reservation cannot be longer than 4 hours",
             new[] { nameof(EndTime) });
        }

     // Validate business hours (9:00 - 23:00)
    if (StartTime.Hour < 9 || EndTime.Hour >= 23)
 {
       yield return new ValidationResult(
     "Reservations are only allowed between 9:00 and 23:00",
          new[] { nameof(StartTime), nameof(EndTime) });
      }

   // Don't allow reservations in the past
      if (StartTime < DateTime.Now)
   {
         yield return new ValidationResult(
             "Cannot create reservations in the past",
          new[] { nameof(StartTime) });
        }
    }

    /// <summary>
    /// Checks if this reservation conflicts with another
    /// </summary>
    public bool ConflictsWith(Reservation other)
  {
        if (other == null || other.TableId != TableId) 
            return false;

        if (other.Status == ReservationStatus.Cancelled || 
 other.Status == ReservationStatus.NoShow)
        return false;

        // Check if date ranges overlap
        return (StartTime <= other.EndTime && EndTime >= other.StartTime);
    }
}