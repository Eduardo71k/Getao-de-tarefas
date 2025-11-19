using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Models;
using TaskManagementSystem.Patterns.Notification;
using TaskManagementSystem.Patterns.Observer;
using TaskManagementSystem.Services;
using TaskManagementSystem.Repository;

namespace TaskManagementSystem
{
    class Program
    {
        private static ITaskService _taskService;
        private static NotificationStrategyFactory _strategyFactory;
        private static int _currentUserId = 1;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Sistema de Gestão de Tarefas Colaborativas");
            Console.WriteLine("=============================================\n");

            var serviceProvider = ConfigureServices();
            _taskService = serviceProvider.GetService<ITaskService>();
            _strategyFactory = serviceProvider.GetService<NotificationStrategyFactory>();

            var taskObservable = serviceProvider.GetService<TaskObservable>();
            var notificationService = serviceProvider.GetService<INotificationService>();
            var notificationObserver = new NotificationObserver(notificationService);
            taskObservable.Subscribe(notificationObserver);

            await ShowMainMenu();
        }

        static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddScoped<ITaskService, TaskService>()
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<ITaskRepository, InMemoryTaskRepository>()
                .AddSingleton<TaskObservable>()
                .AddScoped<NotificationStrategyFactory>()
                .BuildServiceProvider();
        }

        static async Task ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(" MENU PRINCIPAL - Sistema de Tarefas");
                Console.WriteLine("======================================");
                Console.WriteLine($" Usuário Atual: {_currentUserId}");
                Console.WriteLine();
                Console.WriteLine("1. Criar Nova Tarefa");
                Console.WriteLine("2. Listar Minhas Tarefas");
                Console.WriteLine("3. Listar Todas as Tarefas");
                Console.WriteLine("4. Atualizar Status da Tarefa");
                Console.WriteLine("5. Atribuir Tarefa a Outro Usuário");
                Console.WriteLine("6. Testar Notificações");
                Console.WriteLine("7. Alterar Usuário Atual");
                Console.WriteLine("8. Sair");
                Console.WriteLine();
                Console.Write("Digite sua opção (1-8): ");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await CreateTask();
                        break;
                    case "2":
                        await ListMyTasks();
                        break;
                    case "3":
                        await ListAllTasks();
                        break;
                    case "4":
                        await UpdateTaskStatus();
                        break;
                    case "5":
                        await AssignTask();
                        break;
                    case "6":
                        await TestNotifications();
                        break;
                    case "7":
                        ChangeCurrentUser();
                        break;
                    case "8":
                        Console.WriteLine(" Saindo do sistema...");
                        return;
                    default:
                        Console.WriteLine(" Opção inválida! Pressione qualquer tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static async Task CreateTask()
        {
            Console.Clear();
            Console.WriteLine("CRIAR NOVA TAREFA");
            Console.WriteLine("====================");

            try
            {
                Console.Write("Título da tarefa: ");
                var title = Console.ReadLine();

                Console.Write("Descrição: ");
                var description = Console.ReadLine();

                Console.Write("ID do responsável: ");
                if (!int.TryParse(Console.ReadLine(), out int assigneeId))
                {
                    Console.WriteLine(" ID inválido!");
                    PressToContinue();
                    return;
                }

                Console.Write("Data de vencimento (dd/mm/aaaa): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
                {
                    Console.WriteLine(" Data inválida! Usando data padrão.");
                    dueDate = DateTime.Now.AddDays(7);
                }

                var newTask = new TaskItem
                {
                    Title = title,
                    Description = description,
                    AssigneeId = assigneeId,
                    CreatorId = _currentUserId,
                    DueDate = dueDate,
                    Status = TaskItemStatus.Pending
                };

                var createdTask = await _taskService.CreateTaskAsync(newTask);
                Console.WriteLine($" Tarefa criada com sucesso! ID: {createdTask.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Erro ao criar tarefa: {ex.Message}");
            }

            PressToContinue();
        }

        static async Task ListMyTasks()
        {
            Console.Clear();
            Console.WriteLine(" MINHAS TAREFAS");
            Console.WriteLine("=================");

            try
            {
                var tasks = await _taskService.GetUserTasksAsync(_currentUserId);

                if (!tasks.Any())
                {
                    Console.WriteLine("Nenhuma tarefa encontrada.");
                }
                else
                {
                    foreach (var task in tasks)
                    {
                        Console.WriteLine($"\n ID: {task.Id}");
                        Console.WriteLine($"   Título: {task.Title}");
                        Console.WriteLine($"   Descrição: {task.Description}");
                        Console.WriteLine($"   Status: {task.Status}");
                        Console.WriteLine($"   Vencimento: {task.DueDate:dd/MM/yyyy}");
                        Console.WriteLine($"   Criador: {task.CreatorId}");
                        if (task.CompletedAt.HasValue)
                            Console.WriteLine($"   Concluída em: {task.CompletedAt:dd/MM/yyyy}");
                        Console.WriteLine("   " + new string('-', 30));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Erro ao listar tarefas: {ex.Message}");
            }

            PressToContinue();
        }

        static async Task ListAllTasks()
        {
            Console.Clear();
            Console.WriteLine(" TODAS AS TAREFAS");
            Console.WriteLine("===================");

            try
            {
                var tasks = await _taskService.GetAllTasksAsync();

                if (!tasks.Any())
                {
                    Console.WriteLine("Nenhuma tarefa encontrada.");
                }
                else
                {
                    foreach (var task in tasks)
                    {
                        var statusIcon = task.Status switch
                        {
                            TaskItemStatus.Pending => " pendente",
                            TaskItemStatus.InProgress => " progresso",
                            TaskItemStatus.Completed => " completo",
                            _ => " "
                        };

                        Console.WriteLine($"\n{statusIcon} ID: {task.Id}");
                        Console.WriteLine($"   Título: {task.Title}");
                        Console.WriteLine($"   Responsável: {task.AssigneeId}");
                        Console.WriteLine($"   Status: {task.Status}");
                        Console.WriteLine($"   Vencimento: {task.DueDate:dd/MM/yyyy}");
                        Console.WriteLine("   " + new string('-', 30));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Erro ao listar tarefas: {ex.Message}");
            }

            PressToContinue();
        }

        static async Task UpdateTaskStatus()
        {
            Console.Clear();
            Console.WriteLine(" ATUALIZAR STATUS DA TAREFA");
            Console.WriteLine("=============================");

            try
            {
                Console.Write("ID da tarefa: ");
                if (!int.TryParse(Console.ReadLine(), out int taskId))
                {
                    Console.WriteLine(" ID inválido!");
                    PressToContinue();
                    return;
                }

                Console.WriteLine("\nSelecione o novo status:");
                Console.WriteLine("1. Pendente");
                Console.WriteLine("2. Em Andamento");
                Console.WriteLine("3. Concluída");
                Console.Write("Opção (1-3): ");

                var statusInput = Console.ReadLine();
                TaskItemStatus newStatus = statusInput switch
                {
                    "1" => TaskItemStatus.Pending,
                    "2" => TaskItemStatus.InProgress,
                    "3" => TaskItemStatus.Completed,
                    _ => throw new ArgumentException("Status inválido")
                };

                var updatedTask = await _taskService.UpdateTaskStatusAsync(taskId, newStatus);
                Console.WriteLine($" Status atualizado para: {updatedTask.Status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Erro ao atualizar status: {ex.Message}");
            }

            PressToContinue();
        }

        static async Task AssignTask()
        {
            Console.Clear();
            Console.WriteLine(" ATRIBUIR TAREFA A OUTRO USUÁRIO");
            Console.WriteLine("==================================");

            try
            {
                Console.Write("ID da tarefa: ");
                if (!int.TryParse(Console.ReadLine(), out int taskId))
                {
                    Console.WriteLine(" ID inválido!");
                    PressToContinue();
                    return;
                }

                Console.Write("Novo ID do responsável: ");
                if (!int.TryParse(Console.ReadLine(), out int newAssigneeId))
                {
                    Console.WriteLine(" ID inválido!");
                    PressToContinue();
                    return;
                }

                var updatedTask = await _taskService.AssignTaskAsync(taskId, newAssigneeId);
                Console.WriteLine($" Tarefa '{updatedTask.Title}' atribuída ao usuário {newAssigneeId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Erro ao atribuir tarefa: {ex.Message}");
            }

            PressToContinue();
        }

        static async Task TestNotifications()
        {
            Console.Clear();
            Console.WriteLine(" TESTAR NOTIFICAÇÕES");
            Console.WriteLine("======================");

            try
            {
                Console.WriteLine("Selecione o tipo de notificação:");
                Console.WriteLine("1. Email");
                Console.WriteLine("2. Push Notification");
                Console.Write("Opção (1-2): ");

                var typeInput = Console.ReadLine();
                NotificationType notificationType = typeInput switch
                {
                    "1" => NotificationType.Email,
                    "2" => NotificationType.Push,
                    _ => throw new ArgumentException("Tipo inválido")
                };

                Console.Write("ID do usuário destinatário: ");
                if (!int.TryParse(Console.ReadLine(), out int userId))
                {
                    Console.WriteLine(" ID inválido!");
                    PressToContinue();
                    return;
                }

                Console.Write("Mensagem: ");
                var message = Console.ReadLine();

                var strategy = _strategyFactory.CreateStrategy(notificationType);
                await strategy.SendNotificationAsync(message, userId);

                Console.WriteLine(" Notificação enviada com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Erro ao enviar notificação: {ex.Message}");
            }

            PressToContinue();
        }

        static void ChangeCurrentUser()
        {
            Console.Clear();
            Console.WriteLine(" ALTERAR USUÁRIO ATUAL");
            Console.WriteLine("========================");

            Console.Write("Novo ID do usuário: ");
            if (int.TryParse(Console.ReadLine(), out int newUserId))
            {
                _currentUserId = newUserId;
                Console.WriteLine($" Usuário alterado para: {_currentUserId}");
            }
            else
            {
                Console.WriteLine(" ID inválido! Mantendo usuário atual.");
            }

            PressToContinue();
        }

        static void PressToContinue()
        {
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
}