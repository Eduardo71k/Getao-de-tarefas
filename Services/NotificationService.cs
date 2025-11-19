namespace TaskManagementSystem.Services
{
    public class NotificationService : INotificationService
    {
        public async Task NotifyUserAsync(int userId, string message)
        {
            Console.WriteLine($" Notificação para usuário {userId}: {message}");
            await Task.Delay(100);
        }
    }
}
