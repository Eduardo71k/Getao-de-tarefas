namespace TaskManagementSystem.Patterns.Notification
{
    public enum NotificationType
    {
        Email,
        Push
    }

    public class NotificationStrategyFactory
    {
        public INotificationStrategy CreateStrategy(NotificationType type)
        {
            return type switch
            {
                NotificationType.Email => new EmailNotificationStrategy(),
                NotificationType.Push => new PushNotificationStrategy(),
                _ => throw new ArgumentException("Tipo de notificação não suportado")
            };
        }
    }
}