using RestaurantSystem.Core.Common;
using RestaurantSystem.Core.Models;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.Core.Interfaces.Repositories;

public interface ITableRepository : IRepository<Table>
{
    Task<Result<IEnumerable<Table>>> GetAvailableTablesAsync(DateTime dateTime);
    Task<Result<IEnumerable<Table>>> GetTablesByLocationAsync(TableLocation location);
    Task<Result<bool>> IsTableAvailableAsync(int tableId, DateTime startTime, DateTime endTime);
}

public interface IReservationRepository : IRepository<Reservation>
{
    Task<Result<IEnumerable<Reservation>>> GetReservationsByDateAsync(DateTime date);
    Task<Result<IEnumerable<Reservation>>> GetReservationsByTableAsync(int tableId);
    Task<Result<IEnumerable<Reservation>>> GetConflictingReservationsAsync(int tableId, DateTime startTime, DateTime endTime);
}

public interface IDishRepository : IRepository<Dish>
{
    Task<Result<IEnumerable<Dish>>> GetDishesByCategoryAsync(DishCategory category);
    Task<Result<IEnumerable<Dish>>> GetAvailableDishesAsync();
    Task<Result<IEnumerable<Dish>>> SearchDishesAsync(string searchTerm);
}

public interface IOrderRepository : IRepository<Order>
{
    Task<Result<IEnumerable<Order>>> GetActiveOrdersAsync();
    Task<Result<IEnumerable<Order>>> GetOrdersByStatusAsync(OrderStatus status);
    Task<Result<IEnumerable<Order>>> GetOrdersByTableAsync(int tableId);
    Task<Result<IEnumerable<Order>>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
}

public interface IUserRepository : IRepository<User>
{
    Task<Result<User?>> GetByUsernameAsync(string username);
    Task<Result<IEnumerable<User>>> GetActiveUsersAsync();
    Task<Result<IEnumerable<User>>> GetUsersByRoleAsync(UserRole role);
}