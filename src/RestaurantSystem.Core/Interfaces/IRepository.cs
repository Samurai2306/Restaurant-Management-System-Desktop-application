using RestaurantSystem.Core.Common;

namespace RestaurantSystem.Core.Interfaces;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
public interface IRepository<T> where T : class
{
    Task<Result<T>> GetByIdAsync(int id);
    Task<Result<IEnumerable<T>>> GetAllAsync();
    Task<Result<T>> AddAsync(T entity);
    Task<Result> UpdateAsync(T entity);
    Task<Result> DeleteAsync(int id);
    Task<Result> SaveChangesAsync();
}