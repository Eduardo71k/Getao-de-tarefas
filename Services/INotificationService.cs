namespace TaskManagementSystem.Services
{
    public interface INotificationService
    {
        Task NotifyUserAsync(int userId, string message);
    }
}