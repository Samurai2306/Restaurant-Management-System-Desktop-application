using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.Core.Models;

public class OrderItem : BaseEntity
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public int DishId { get; set; }

    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, 10000)]
    public decimal UnitPrice { get; set; }

    [StringLength(500)]
    public string SpecialInstructions { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; }
    public virtual Dish Dish { get; set; }

    // Computed properties
    public decimal TotalPrice => Quantity * UnitPrice;

    public OrderItem()
    {
        Status = OrderStatus.New;
   Quantity = 1;
    }

    /// <summary>
    /// Validates if the order item can be cancelled
    /// </summary>
    public bool CanCancel()
    {
    return Status == OrderStatus.New || Status == OrderStatus.InProgress;
}

    /// <summary>
    /// Validates if the status can be changed to the new status
    /// </summary>
    public bool CanChangeStatusTo(OrderStatus newStatus)
    {
        if (Status == newStatus) return true;

 return newStatus switch
        {
          OrderStatus.New => false, // Cannot go back to new
OrderStatus.InProgress => Status == OrderStatus.New,
       OrderStatus.Ready => Status == OrderStatus.InProgress,
     OrderStatus.Served => Status == OrderStatus.Ready,
     OrderStatus.Cancelled => CanCancel(),
            _ => false
        };
    }

    /// <summary>
    /// Changes the status of the order item if possible
    /// </summary>
    public void ChangeStatus(OrderStatus newStatus)
    {
     if (!CanChangeStatusTo(newStatus))
            throw new InvalidOperationException($"Cannot change status from {Status} to {newStatus}");

        Status = newStatus;
    }
}