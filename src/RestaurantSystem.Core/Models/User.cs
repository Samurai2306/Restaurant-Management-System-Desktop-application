using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.Core.Models;

/// <summary>
/// User model representing system users
/// </summary>
public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ProfilePicturePath { get; set; }
    public DateTime? LastLogin { get; set; }

    // Navigation properties
    public virtual ICollection<Order>? CreatedOrders { get; set; }
    public virtual ICollection<Reservation>? CreatedReservations { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public bool IsAdmin => Role == UserRole.Admin;
}
