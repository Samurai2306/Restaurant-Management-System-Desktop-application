using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.Core.Models;
using RestaurantSystem.Core.Common;
using RestaurantSystem.Data.Context;

namespace RestaurantSystem.Data.Repositories;

/// <summary>
/// Repository for User operations
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Result<User?>> GetByUsernameAsync(string username)
    {
        try
        {
            var user = await _dbSet
                .Where(u => u.Username == username && !u.IsDeleted)
                .FirstOrDefaultAsync();

            return Result<User?>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User?>.Failure($"Error getting user by username: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<User>>> GetActiveUsersAsync()
    {
        try
        {
            var users = await _dbSet
                .Where(u => u.IsActive && !u.IsDeleted)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            return Result<IEnumerable<User>>.Success(users);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<User>>.Failure($"Error getting active users: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<User>>> GetUsersByRoleAsync(Core.Enums.UserRole role)
    {
        try
        {
            var users = await _dbSet
                .Where(u => u.Role == role && !u.IsDeleted)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            return Result<IEnumerable<User>>.Success(users);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<User>>.Failure($"Error getting users by role: {ex.Message}");
        }
    }
}

