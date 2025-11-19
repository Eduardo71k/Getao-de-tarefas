using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services
{
    public interface ITaskService
    {
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem> UpdateTaskStatusAsync(int taskId, TaskItemStatus status);
        Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId);
        Task<TaskItem> AssignTaskAsync(int taskId, int assigneeId);
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    }
}