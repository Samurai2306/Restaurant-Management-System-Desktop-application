using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Core.Common;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.Core.Models;
using RestaurantSystem.Data.Context;

namespace RestaurantSystem.Data.Repositories;

public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context) : base(context)
    {
 _context = context;
    }

    public async Task<Result<IEnumerable<Reservation>>> GetReservationsByDateAsync(DateTime date)
    {
        try
        {
 var reservations = await _context.Reservations
      .Include(r => r.Table)
   .Where(r => r.StartTime.Date == date.Date)
        .OrderBy(r => r.StartTime)
    .ToListAsync();

   return Result<IEnumerable<Reservation>>.Success(reservations);
        }
        catch (Exception ex)
  {
       return Result<IEnumerable<Reservation>>.Failure($"Error getting reservations by date: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Reservation>>> GetReservationsByTableAsync(int tableId)
 {
   try
        {
   var reservations = await _context.Reservations
    .Include(r => r.Table)
   .Where(r => r.TableId == tableId)
  .OrderBy(r => r.StartTime)
             .ToListAsync();

   return Result<IEnumerable<Reservation>>.Success(reservations);
      }
   catch (Exception ex)
     {
       return Result<IEnumerable<Reservation>>.Failure($"Error getting reservations for table: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Reservation>>> GetConflictingReservationsAsync(
     int tableId, 
      DateTime startTime, 
    DateTime endTime)
    {
      try
        {
    var conflictingReservations = await _context.Reservations
      .Include(r => r.Table)
     .Where(r => r.TableId == tableId &&
        r.Status != ReservationStatus.Cancelled &&
     r.Status != ReservationStatus.NoShow &&
        !(endTime <= r.StartTime || startTime >= r.EndTime))
         .OrderBy(r => r.StartTime)
     .ToListAsync();

            return Result<IEnumerable<Reservation>>.Success(conflictingReservations);
        }
        catch (Exception ex)
        {
  return Result<IEnumerable<Reservation>>.Failure($"Error checking conflicting reservations: {ex.Message}");
        }
    }
}