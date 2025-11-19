using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManagementSystem.Patterns.Notification;

namespace TaskManagementSystem.Tests
{
    [TestClass]
    public class NotificationStrategyTests
    {
        [TestMethod]
        public async Task EmailNotificationStrategy_SendNotification_ExecutesSuccessfully()
        {
            var strategy = new EmailNotificationStrategy();

            await strategy.SendNotificationAsync("Test message", 1);
        }

        [TestMethod]
        public async Task PushNotificationStrategy_SendNotification_ExecutesSuccessfully()
        {
            var strategy = new PushNotificationStrategy();

            await strategy.SendNotificationAsync("Test message", 1);
        }

        [TestMethod]
        public void NotificationStrategyFactory_CreateEmailStrategy_ReturnsCorrectType()
        {
            var factory = new NotificationStrategyFactory();

            var strategy = factory.CreateStrategy(NotificationType.Email);

            Assert.IsInstanceOfType(strategy, typeof(EmailNotificationStrategy));
        }

        [TestMethod]
        public void NotificationStrategyFactory_CreatePushStrategy_ReturnsCorrectType()
        {
            var factory = new NotificationStrategyFactory();

            var strategy = factory.CreateStrategy(NotificationType.Push);

            Assert.IsInstanceOfType(strategy, typeof(PushNotificationStrategy));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NotificationStrategyFactory_InvalidType_ThrowsException()
        {
            var factory = new NotificationStrategyFactory();

            factory.CreateStrategy((NotificationType)999);
        }
    }
}