using RestaurantSystem.Core.Common;

namespace RestaurantSystem.Core.Interfaces.Services;

/// <summary>
/// Interface for notification service
/// </summary>
public interface INotificationService
{
    Task<Result> SendNotificationAsync(string recipient, string subject, string message);
    Task<Result> SendReservationReminderAsync(int reservationId);
    Task<Result> SendOrderStatusUpdateAsync(int orderId, string status);
}

/// <summary>
/// Interface for export service
/// </summary>
public interface IExportService
{
    Task<Result<byte[]>> ExportToPdfAsync(string templateName, object data);
    Task<Result<byte[]>> ExportToExcelAsync(string templateName, object data);
    Task<Result<byte[]>> ExportToCsvAsync(string templateName, object data);
}

/// <summary>
/// Interface for logging service
/// </summary>
public interface ILoggingService
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, Exception exception = null, params object[] args);
    void LogDebug(string message, params object[] args);
}