using System.Security.Cryptography;
using System.Text;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.Core.Interfaces.Services;
using RestaurantSystem.Core.Models;
using RestaurantSystem.Core.Common;

namespace RestaurantSystem.UI.Services;

/// <summary>
/// Service for authentication and authorization
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private User? _currentUser;

    public User? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser != null;
    public bool IsAdmin => _currentUser?.IsAdmin ?? false;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var result = await _userRepository.GetByUsernameAsync(username);
        
        if (!result.Succeeded || result.Value == null)
        {
            Console.WriteLine($"User not found: {username}");
            return null;
        }

        var user = result.Value;
        
        Console.WriteLine($"Attempting login for: {username}");
        Console.WriteLine($"Stored hash: {user.PasswordHash}");
        
        var inputHash = HashPassword(password);
        Console.WriteLine($"Input hash: {inputHash}");

        // Check password
        if (!VerifyPassword(password, user.PasswordHash))
        {
            Console.WriteLine("Password verification FAILED");
            return null;
        }

        Console.WriteLine("Password verification SUCCESS");

        // Update last login
        user.LastLogin = DateTime.Now;
        await _userRepository.SaveChangesAsync();

        _currentUser = user;
        return user;
    }

    public async Task<bool> RegisterAsync(User user, string password)
    {
        Console.WriteLine($"Attempting to register user: {user.Username}");
        
        // Check if user exists
        var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
        if (existingUser.Succeeded && existingUser.Value != null)
        {
            Console.WriteLine($"User already exists: {user.Username}");
            return false;
        }

        // Hash password
        user.PasswordHash = HashPassword(password);
        Console.WriteLine($"Password hash for new user: {user.PasswordHash}");

        // Add user
        var result = await _userRepository.AddAsync(user);
        if (!result.Succeeded)
        {
            Console.WriteLine($"Failed to add user: {string.Join(", ", result.Errors)}");
            return false;
        }

        await _userRepository.SaveChangesAsync();
        Console.WriteLine($"User registered successfully: {user.Username}");
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var result = await _userRepository.GetByIdAsync(userId);
        if (!result.Succeeded || result.Value == null)
        {
            return false;
        }

        var user = result.Value;

        // Verify old password
        if (!VerifyPassword(oldPassword, user.PasswordHash))
        {
            return false;
        }

        // Update password
        user.PasswordHash = HashPassword(newPassword);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateProfileAsync(User user)
    {
        var result = await _userRepository.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return false;
        }

        await _userRepository.SaveChangesAsync();
        return true;
    }

    public void Logout()
    {
        _currentUser = null;
    }

    public Task<bool> HasPermissionAsync(string permission)
    {
        if (!IsAuthenticated)
        {
            return Task.FromResult(false);
        }

        // Simple permission check based on role
        // Can be extended for more granular permissions
        bool hasPermission = _currentUser!.Role switch
        {
            Core.Enums.UserRole.Admin => true, // Admin has all permissions
            Core.Enums.UserRole.Manager => permission != "admin", // Manager has most permissions
            _ => false // Other roles have no permissions for now
        };

        return Task.FromResult(hasPermission);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        var hash = HashPassword(password);
        return hash == passwordHash;
    }
}

