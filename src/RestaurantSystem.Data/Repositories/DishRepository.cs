using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Core.Common;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.Core.Models;
using RestaurantSystem.Data.Context;

namespace RestaurantSystem.Data.Repositories;

public class DishRepository : GenericRepository<Dish>, IDishRepository
{
    private readonly AppDbContext _context;

    public DishRepository(AppDbContext context) : base(context)
    {
   _context = context;
    }

    public async Task<Result<IEnumerable<Dish>>> GetDishesByCategoryAsync(DishCategory category)
    {
     try
  {
      var dishes = await _context.Dishes
         .Where(d => d.Category == category)
  .OrderBy(d => d.Name)
          .ToListAsync();

      return Result<IEnumerable<Dish>>.Success(dishes);
        }
   catch (Exception ex)
  {
    return Result<IEnumerable<Dish>>.Failure($"Error getting dishes by category: {ex.Message}");
      }
    }

    public async Task<Result<IEnumerable<Dish>>> GetAvailableDishesAsync()
    {
 try
  {
  var dishes = await _context.Dishes
         .Where(d => d.IsAvailable)
  .OrderBy(d => d.Category)
  .ThenBy(d => d.Name)
    .ToListAsync();

    return Result<IEnumerable<Dish>>.Success(dishes);
      }
     catch (Exception ex)
{
      return Result<IEnumerable<Dish>>.Failure($"Error getting available dishes: {ex.Message}");
     }
 }

    public async Task<Result<IEnumerable<Dish>>> SearchDishesAsync(string searchTerm)
    {
        try
   {
          if (string.IsNullOrWhiteSpace(searchTerm))
       return Result<IEnumerable<Dish>>.Success(Array.Empty<Dish>());

         searchTerm = searchTerm.ToLower();

    var dishes = await _context.Dishes
       .Where(d => d.Name.ToLower().Contains(searchTerm) ||
        d.Description.ToLower().Contains(searchTerm) ||
           d.Tags.ToLower().Contains(searchTerm))
                .OrderBy(d => d.Name)
      .ToListAsync();

            return Result<IEnumerable<Dish>>.Success(dishes);
        }
    catch (Exception ex)
        {
            return Result<IEnumerable<Dish>>.Failure($"Error searching dishes: {ex.Message}");
  }
    }
}