using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repository
{
    public interface ITaskRepository
    {
        Task<TaskItem> AddAsync(TaskItem task);
        Task<TaskItem> GetByIdAsync(int id);
        Task<TaskItem> UpdateAsync(TaskItem task);
        Task<IEnumerable<TaskItem>> GetByAssigneeAsync(int assigneeId);
        Task<IEnumerable<TaskItem>> GetAllAsync();
    }
}