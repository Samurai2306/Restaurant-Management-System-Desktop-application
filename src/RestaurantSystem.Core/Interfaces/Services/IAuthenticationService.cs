using RestaurantSystem.Core.Models;

namespace RestaurantSystem.Core.Interfaces.Services;

/// <summary>
/// Interface for authentication and authorization services
/// </summary>
public interface IAuthenticationService
{
    Task<User?> LoginAsync(string username, string password);
    Task<bool> RegisterAsync(User user, string password);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    Task<bool> UpdateProfileAsync(User user);
    User? CurrentUser { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    void Logout();
    Task<bool> HasPermissionAsync(string permission);
}

