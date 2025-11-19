using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repository
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _tasks = new();
        private int _nextId = 1;

        public Task<TaskItem> AddAsync(TaskItem task)
        {
            task.Id = _nextId++;
            _tasks.Add(task);
            return Task.FromResult(task);
        }

        public Task<TaskItem> GetByIdAsync(int id)
        {
            return Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));
        }

        public Task<TaskItem> UpdateAsync(TaskItem task)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing != null)
            {
                _tasks.Remove(existing);
                _tasks.Add(task);
                return Task.FromResult(task);
            }
            throw new ArgumentException("Tarefa não encontrada");
        }

        public Task<IEnumerable<TaskItem>> GetByAssigneeAsync(int assigneeId)
        {
            return Task.FromResult(_tasks.Where(t => t.AssigneeId == assigneeId).AsEnumerable());
        }

        public Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return Task.FromResult(_tasks.AsEnumerable());
        }
    }
}