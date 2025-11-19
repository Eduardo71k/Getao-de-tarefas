using TaskManagementSystem.Models;

namespace TaskManagementSystem.Patterns.Observer
{
    public class TaskObservable
    {
        private readonly List<ITaskObserver> _observers = new();

        public void Subscribe(ITaskObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(ITaskObserver observer)
        {
            _observers.Remove(observer);
        }

        public async Task NotifyStatusChanged(TaskItem task, TaskItemStatus oldStatus)
        {
            foreach (var observer in _observers)
            {
                await observer.OnTaskStatusChanged(task, oldStatus);
            }
        }

        public async Task NotifyTaskAssigned(TaskItem task, int previousAssigneeId)
        {
            foreach (var observer in _observers)
            {
                await observer.OnTaskAssigned(task, previousAssigneeId);
            }
        }
    }
}