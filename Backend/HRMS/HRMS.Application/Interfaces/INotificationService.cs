using HRMS.Core.Entities.Notifications;

namespace HRMS.Application.Interfaces;

public interface INotificationService
{
    Task SendAsync(string userId, string title, string message, string type = "Info", string? referenceType = null, string? referenceId = null);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(string userId);
    Task<List<Notification>> GetUserNotificationsAsync(string userId, int count = 20);
    Task<int> GetUnreadCountAsync(string userId);
}
