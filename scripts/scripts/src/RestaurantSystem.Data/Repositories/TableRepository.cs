using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Core.Common;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.Core.Models;
using RestaurantSystem.Data.Context;

namespace RestaurantSystem.Data.Repositories;

public class TableRepository : GenericRepository<Table>, ITableRepository
{
    private readonly AppDbContext _context;

    public TableRepository(AppDbContext context) : base(context)
    {
   _context = context;
    }

public async Task<Result<IEnumerable<Table>>> GetAvailableTablesAsync(DateTime dateTime)
    {
      try
        {
            var tables = await _context.Tables
      .Include(t => t.Reservations)
                .Include(t => t.Orders)
                .Where(t => t.IsActive)
    .ToListAsync();

     var availableTables = tables.Where(t => t.IsAvailable(dateTime));
            return Result<IEnumerable<Table>>.Success(availableTables);
        }
        catch (Exception ex)
      {
            return Result<IEnumerable<Table>>.Failure($"Error getting available tables: {ex.Message}");
    }
    }

    public async Task<Result<IEnumerable<Table>>> GetTablesByLocationAsync(TableLocation location)
    {
      try
        {
        var tables = await _context.Tables
          .Where(t => t.Location == location)
              .ToListAsync();

   return Result<IEnumerable<Table>>.Success(tables);
        }
     catch (Exception ex)
        {
          return Result<IEnumerable<Table>>.Failure($"Error getting tables by location: {ex.Message}");
        }
    }

    public async Task<Result<bool>> IsTableAvailableAsync(int tableId, DateTime startTime, DateTime endTime)
    {
        try
    {
            var table = await _context.Tables
           .Include(t => t.Reservations)
       .Include(t => t.Orders)
 .FirstOrDefaultAsync(t => t.Id == tableId);

    if (table == null)
                return Result<bool>.Failure($"Table with id {tableId} not found");

            // Check active reservations during the time period
      bool hasConflictingReservation = table.Reservations.Any(r =>
 r.Status != ReservationStatus.Cancelled &&
  r.Status != ReservationStatus.NoShow &&
    !(endTime <= r.StartTime || startTime >= r.EndTime));

            if (hasConflictingReservation)
     return Result<bool>.Success(false);

            // Check active orders
     bool hasActiveOrder = table.Orders.Any(o =>
           o.Status != OrderStatus.Paid &&
    o.Status != OrderStatus.Cancelled &&
      o.CreatedTime <= endTime &&
   (!o.ClosedTime.HasValue || o.ClosedTime.Value >= startTime));

        return Result<bool>.Success(!hasActiveOrder);
        }
        catch (Exception ex)
        {
   return Result<bool>.Failure($"Error checking table availability: {ex.Message}");
        }
    }
}