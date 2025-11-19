namespace TaskManagementSystem.Patterns.Notification
{
    public interface INotificationStrategy
    {
        Task SendNotificationAsync(string message, int userId);
    }
}