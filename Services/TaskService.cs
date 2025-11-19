using TaskManagementSystem.Models;
using TaskManagementSystem.Repository;
using TaskManagementSystem.Patterns.Observer;

namespace TaskManagementSystem.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly INotificationService _notificationService;
        private readonly TaskObservable _taskObservable;

        public TaskService(ITaskRepository taskRepository,
                          INotificationService notificationService,
                          TaskObservable taskObservable)
        {
            _taskRepository = taskRepository;
            _notificationService = notificationService;
            _taskObservable = taskObservable;
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            var createdTask = await _taskRepository.AddAsync(task);

            await _notificationService.NotifyUserAsync(
                task.AssigneeId,
                $"Nova tarefa atribuída: {task.Title}"
            );

            return createdTask;
        }

        public async Task<TaskItem> UpdateTaskStatusAsync(int taskId, TaskItemStatus status)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new ArgumentException("Tarefa não encontrada");

            var oldStatus = task.Status;
            task.Status = status;

            if (status == TaskItemStatus.Completed)
                task.CompletedAt = DateTime.UtcNow;

            var updatedTask = await _taskRepository.UpdateAsync(task);

            await _taskObservable.NotifyStatusChanged(updatedTask, oldStatus);

            return updatedTask;
        }

        public async Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId)
        {
            return await _taskRepository.GetByAssigneeAsync(userId);
        }

        public async Task<TaskItem> AssignTaskAsync(int taskId, int assigneeId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new ArgumentException("Tarefa não encontrada");

            var previousAssignee = task.AssigneeId;
            task.AssigneeId = assigneeId;

            var updatedTask = await _taskRepository.UpdateAsync(task);

            await _taskObservable.NotifyTaskAssigned(updatedTask, previousAssignee);

            return updatedTask;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _taskRepository.GetAllAsync();
        }
    }
}