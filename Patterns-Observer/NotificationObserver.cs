using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.Patterns.Observer
{
    public class NotificationObserver : ITaskObserver
    {
        private readonly INotificationService _notificationService;

        public NotificationObserver(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task OnTaskStatusChanged(TaskItem task, TaskItemStatus oldStatus)
        {
            var message = $"Status da tarefa '{task.Title}' alterado de {oldStatus} para {task.Status}";
            await _notificationService.NotifyUserAsync(task.CreatorId, message);

            if (task.Status == TaskItemStatus.Completed)
            {
                await _notificationService.NotifyUserAsync(
                    task.AssigneeId,
                    $" Parabéns! Você completou a tarefa: {task.Title}"
                );
            }
        }

        public async Task OnTaskAssigned(TaskItem task, int previousAssigneeId)
        {
            if (previousAssigneeId != task.AssigneeId)
            {
                var message = $" Você foi designado para a tarefa: {task.Title}";
                await _notificationService.NotifyUserAsync(task.AssigneeId, message);

                if (previousAssigneeId > 0)
                {
                    await _notificationService.NotifyUserAsync(
                        previousAssigneeId,
                        $" Você não é mais responsável pela tarefa: {task.Title}"
                    );
                }
            }
        }
    }
}