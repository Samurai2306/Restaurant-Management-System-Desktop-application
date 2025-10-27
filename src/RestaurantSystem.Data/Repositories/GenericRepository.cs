using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Core.Interfaces;
using RestaurantSystem.Core.Common;

namespace RestaurantSystem.Data.Repositories;

/// <summary>
/// Generic repository implementation
/// </summary>
public class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext context)
    {
    _context = context;
        _dbSet = context.Set<T>();
    }

  public virtual async Task<Result<T>> GetByIdAsync(int id)
    {
        try
   {
   var entity = await _dbSet.FindAsync(id);
  if (entity == null)
                return Result<T>.Failure($"Entity of type {typeof(T).Name} with id {id} was not found.");
      
      return Result<T>.Success(entity);
        }
        catch (Exception ex)
        {
       return Result<T>.Failure($"Error getting entity by id: {ex.Message}");
        }
}

    public virtual async Task<Result<IEnumerable<T>>> GetAllAsync()
    {
        try
        {
  var entities = await _dbSet.ToListAsync();
            return Result<IEnumerable<T>>.Success(entities);
 }
 catch (Exception ex)
        {
            return Result<IEnumerable<T>>.Failure($"Error getting all entities: {ex.Message}");
        }
    }

    public virtual async Task<Result<T>> AddAsync(T entity)
    {
        try
        {
            var result = await _dbSet.AddAsync(entity);
         return Result<T>.Success(result.Entity);
        }
        catch (Exception ex)
        {
          return Result<T>.Failure($"Error adding entity: {ex.Message}");
        }
}

    public virtual async Task<Result> UpdateAsync(T entity)
    {
        try
        {
            // Get the key property value
            var keyProperty = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
            if (keyProperty == null)
            {
                return Result.Failure("Entity type does not have a primary key defined.");
            }

            // Get the key value from the entity
            var keyValue = keyProperty.Properties.First().PropertyInfo.GetValue(entity);
            if (keyValue == null)
            {
                return Result.Failure("Entity key value is null.");
            }

            // Try to find the existing tracked entity
            var trackedEntity = await _dbSet.FindAsync(keyValue);
            
            if (trackedEntity != null)
            {
                // Copy values from entity to tracked entity
                _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                // Entity not tracked, try to attach
                var entry = _context.Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }
                entry.State = EntityState.Modified;
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error updating entity: {ex.Message}");
        }
    }

    public virtual async Task<Result> DeleteAsync(int id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
      return Result.Failure($"Entity of type {typeof(T).Name} with id {id} was not found.");

     _dbSet.Remove(entity);
        return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting entity: {ex.Message}");
        }
    }

    public virtual async Task<Result> SaveChangesAsync()
    {
        try
     {
        await _context.SaveChangesAsync();
            return Result.Success();
     }
        catch (Exception ex)
        {
          return Result.Failure($"Error saving changes: {ex.Message}");
        }
    }
}