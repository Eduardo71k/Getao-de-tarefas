namespace TaskManagementSystem.Patterns.Notification
{
    public class EmailNotificationStrategy : INotificationStrategy
    {
        public async Task SendNotificationAsync(string message, int userId)
        {
            Console.WriteLine($" Enviando email para usuário {userId}: {message}");
            await Task.Delay(100);
        }
    }
}