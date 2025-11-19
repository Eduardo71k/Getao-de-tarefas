namespace TaskManagementSystem.Patterns.Notification
{
    public class PushNotificationStrategy : INotificationStrategy
    {
        public async Task SendNotificationAsync(string message, int userId)
        {
            Console.WriteLine($"Enviando push notification para usuário {userId}: {message}");
            await Task.Delay(100);
        }
    }
}
