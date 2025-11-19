using TaskManagementSystem.Models;

namespace TaskManagementSystem.Patterns.Observer
{
    public interface ITaskObserver
    {
        Task OnTaskStatusChanged(TaskItem task, TaskItemStatus oldStatus);
        Task OnTaskAssigned(TaskItem task, int previousAssigneeId);
    }
}