using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.Core.Models;

public class Table : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    public TableLocation Location { get; set; }

    [Required]
    [Range(1, 20)]
    public int SeatsCount { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Reservation> Reservations { get; set; }
    public virtual ICollection<Order> Orders { get; set; }

    public Table()
    {
        Reservations = new HashSet<Reservation>();
        Orders = new HashSet<Order>();
    }

    /// <summary>
    /// Checks if the table is available at the specified time
    /// </summary>
    public bool IsAvailable(DateTime dateTime)
    {
     if (!IsActive) return false;

        // Check reservations
      bool hasConflictingReservation = Reservations.Any(r => 
 r.Status != ReservationStatus.Cancelled && 
            r.Status != ReservationStatus.NoShow &&
 dateTime >= r.StartTime && 
         dateTime <= r.EndTime);

        if (hasConflictingReservation) return false;

        // Check active orders
   bool hasActiveOrder = Orders.Any(o => 
         o.Status != OrderStatus.Paid && 
    o.Status != OrderStatus.Cancelled &&
            dateTime >= o.CreatedTime && 
      (o.ClosedTime == null || dateTime <= o.ClosedTime));

 return !hasActiveOrder;
    }

    /// <summary>
    /// Gets current status of the table
    /// </summary>
    public TableStatus GetStatus(DateTime currentTime)
    {
        if (!IsActive) return TableStatus.OutOfService;

        var activeOrder = Orders.FirstOrDefault(o =>
      o.Status != OrderStatus.Paid &&
   o.Status != OrderStatus.Cancelled);

     if (activeOrder != null)
   return TableStatus.Occupied;

        var currentReservation = Reservations.FirstOrDefault(r =>
            r.Status == ReservationStatus.Confirmed &&
 currentTime >= r.StartTime &&
            currentTime <= r.EndTime);

        if (currentReservation != null)
       return TableStatus.Reserved;

        return TableStatus.Available;
    }
}