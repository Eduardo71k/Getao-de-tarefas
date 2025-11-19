using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
using TaskManagementSystem.Repository;
using TaskManagementSystem.Patterns.Observer;

namespace TaskManagementSystem.Tests
{
    [TestClass]
    public class TaskServiceTests
    {
        private Mock<ITaskRepository> _mockTaskRepository;
        private Mock<INotificationService> _mockNotificationService;
        private Mock<TaskObservable> _mockTaskObservable;
        private TaskService _taskService;

        [TestInitialize]
        public void Setup()
        {
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockTaskObservable = new Mock<TaskObservable>();
            _taskService = new TaskService(
                _mockTaskRepository.Object,
                _mockNotificationService.Object,
                _mockTaskObservable.Object
            );
        }

        [TestMethod]
        public async Task CreateTaskAsync_ValidTask_ReturnsCreatedTask()
        {
            var task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                AssigneeId = 1,
                CreatorId = 2
            };

            _mockTaskRepository.Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(task);

            var result = await _taskService.CreateTaskAsync(task);

            Assert.IsNotNull(result);
            Assert.AreEqual("Test Task", result.Title);
            _mockNotificationService.Verify(
                service => service.NotifyUserAsync(1, It.IsAny<string>()),
                Times.Once
            );
        }

        [TestMethod]
        public async Task UpdateTaskStatusAsync_ValidTask_UpdatesStatus()
        {
            var task = new TaskItem
            {
                Id = 1,
                Title = "Test Task",
                Status = TaskItemStatus.Pending,
                CreatorId = 1,
                AssigneeId = 2
            };

            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(task);
            _mockTaskRepository.Setup(repo => repo.UpdateAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync((TaskItem t) => t);

            var result = await _taskService.UpdateTaskStatusAsync(1, TaskItemStatus.Completed);

            Assert.AreEqual(TaskItemStatus.Completed, result.Status);
            Assert.IsNotNull(result.CompletedAt);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateTaskStatusAsync_InvalidTaskId_ThrowsException()
        {
            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((TaskItem)null);

            await _taskService.UpdateTaskStatusAsync(1, TaskItemStatus.Completed);
        }

        [TestMethod]
        public async Task GetUserTasksAsync_ValidUserId_ReturnsUserTasks()
        {
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Task 1", AssigneeId = 1 },
                new TaskItem { Id = 2, Title = "Task 2", AssigneeId = 1 }
            };

            _mockTaskRepository.Setup(repo => repo.GetByAssigneeAsync(1))
                .ReturnsAsync(tasks);

            var result = await _taskService.GetUserTasksAsync(1);

            Assert.AreEqual(2, result.Count());
        }
    }
}