using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.Core.Models;

public class Order : BaseEntity
{
    [Required]
    public int TableId { get; set; }

    [Required]
    public DateTime CreatedTime { get; set; }

    public DateTime? ClosedTime { get; set; }

    [StringLength(500)]
    public string SpecialInstructions { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    public string WaiterId { get; set; }

 // Navigation properties
    public virtual Table Table { get; set; }
    public virtual ICollection<OrderItem> Items { get; set; }

    public Order()
    {
        Items = new HashSet<OrderItem>();
        CreatedTime = DateTime.UtcNow;
     Status = OrderStatus.New;
    }

    // Calculated total amount
    public decimal TotalAmount => Items?.Sum(i => i.TotalPrice) ?? 0m;

    /// <summary>
    /// Estimated time until all items are ready (in minutes)
    /// </summary>
    public int EstimatedWaitingTime
    {
        get
        {
            if (!Items?.Any() ?? true) return 0;

          var maxCookingTime = Items
      .Where(i => i.Status != OrderStatus.Served && i.Status != OrderStatus.Cancelled)
                .Max(i => i.Dish?.CookingTimeMinutes ?? 0);

            return maxCookingTime;
 }
    }

    /// <summary>
    /// Updates the order status based on its items
/// </summary>
    public void UpdateStatus()
    {
        if (!Items?.Any() ?? true)
        {
     Status = OrderStatus.New;
            return;
        }

        if (Items.All(i => i.Status == OrderStatus.Served))
        {
        Status = OrderStatus.Served;
            return;
      }

    if (Items.All(i => i.Status == OrderStatus.Cancelled))
        {
            Status = OrderStatus.Cancelled;
    return;
        }

        if (Items.Any(i => i.Status == OrderStatus.InProgress))
        {
            Status = OrderStatus.InProgress;
       return;
        }

     Status = OrderStatus.New;
    }

    /// <summary>
    /// Validates if the order can be closed
    /// </summary>
    public bool CanClose()
    {
        return Status == OrderStatus.Served && !ClosedTime.HasValue;
    }

    /// <summary>
    /// Closes the order and sets the final status
    /// </summary>
    public void Close()
    {
        if (!CanClose())
        throw new InvalidOperationException("Cannot close order that is not fully served");

        ClosedTime = DateTime.UtcNow;
        Status = OrderStatus.Paid;
    }
}