using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Core.Common;
using RestaurantSystem.Core.Enums;
using RestaurantSystem.Core.Interfaces.Repositories;
using RestaurantSystem.Core.Models;
using RestaurantSystem.Data.Context;

namespace RestaurantSystem.Data.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context) : base(context)
    {
     _context = context;
    }

    public async Task<Result<IEnumerable<Order>>> GetActiveOrdersAsync()
    {
     try
   {
      var activeOrders = await _context.Orders
         .Include(o => o.Table)
     .Include(o => o.Items)
         .ThenInclude(i => i.Dish)
         .Where(o => o.Status != OrderStatus.Paid &&
        o.Status != OrderStatus.Cancelled)
   .OrderByDescending(o => o.CreatedTime)
   .ToListAsync();

     return Result<IEnumerable<Order>>.Success(activeOrders);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Order>>.Failure($"Error getting active orders: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Order>>> GetOrdersByStatusAsync(OrderStatus status)
    {
     try
        {
        var orders = await _context.Orders
   .Include(o => o.Table)
    .Include(o => o.Items)
           .ThenInclude(i => i.Dish)
         .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedTime)
      .ToListAsync();

            return Result<IEnumerable<Order>>.Success(orders);
        }
     catch (Exception ex)
     {
            return Result<IEnumerable<Order>>.Failure($"Error getting orders by status: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Order>>> GetOrdersByTableAsync(int tableId)
   {
        try
  {
         var orders = await _context.Orders
   .Include(o => o.Table)
       .Include(o => o.Items)
    .ThenInclude(i => i.Dish)
.Where(o => o.TableId == tableId)
          .OrderByDescending(o => o.CreatedTime)
          .ToListAsync();

       return Result<IEnumerable<Order>>.Success(orders);
  }
        catch (Exception ex)
 {
        return Result<IEnumerable<Order>>.Failure($"Error getting orders for table: {ex.Message}");
     }
    }

    public async Task<Result<IEnumerable<Order>>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
  {
     try
  {
  var orders = await _context.Orders
           .Include(o => o.Table)
     .Include(o => o.Items)
         .ThenInclude(i => i.Dish)
      .Where(o => o.CreatedTime.Date >= startDate.Date &&
     o.CreatedTime.Date <= endDate.Date)
      .OrderByDescending(o => o.CreatedTime)
            .ToListAsync();

            return Result<IEnumerable<Order>>.Success(orders);
        }
        catch (Exception ex)
  {
     return Result<IEnumerable<Order>>.Failure($"Error getting orders by date range: {ex.Message}");
        }
    }
}