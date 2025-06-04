using Microsoft.Extensions.Logging;
using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;
using SchoolTodoApi.Services;
using System;
using System.Threading.Tasks;

public class ReminderService
{
    private readonly ITodoItemRepository _todoItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly MailService _mailService;
    private readonly ILogger<ReminderService> _logger;

    public ReminderService(
        ITodoItemRepository todoItemRepository,
        IUserRepository userRepository,
        MailService mailService,
        ILogger<ReminderService> logger)
    {
        _todoItemRepository = todoItemRepository;
        _userRepository = userRepository;
        _mailService = mailService;
        _logger = logger;
    }

    public async Task SendUpcomingTodoReminders()
    {
        _logger.LogInformation("Reminder job started at {Time}", DateTime.UtcNow);

        var upcomingTodos = await _todoItemRepository.GetTodosDueInNextDaysAsync(3);
        _logger.LogInformation("Found {Count} todos due in the next 3 days", upcomingTodos.Count);

        foreach (var todo in upcomingTodos)
        {
            var user = await _userRepository.GetByIdAsync(todo.CreatedById);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                string msg = $"Reminder: Your todo '{todo.Title}' is due on {todo.DueDate:yyyy-MM-dd}";
                await _mailService.SendEmail(user.Email, user.Username, msg);

                _logger.LogInformation("Sent reminder for Todo '{Title}' to user {Email}", todo.Title, user.Email);
            }
            else
            {
                _logger.LogWarning("No email found for user ID {UserId}, skipping reminder for Todo '{Title}'", todo.CreatedById, todo.Title);
            }
        }

        _logger.LogInformation("Reminder job completed at {Time}", DateTime.UtcNow);
    }
}
