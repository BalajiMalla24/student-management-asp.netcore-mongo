using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;
using SchoolTodoApi.Services;

public class ReminderService
{
    private readonly ITodoItemRepository _todoItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly MailService _mailService;

    public ReminderService(
        ITodoItemRepository todoItemRepository,
        IUserRepository userRepository,
        MailService mailService)
    {
        _todoItemRepository = todoItemRepository;
        _userRepository = userRepository;
        _mailService = mailService;
    }

    public async Task SendUpcomingTodoReminders()
    {
        var upcomingTodos = await _todoItemRepository.GetTodosDueInNextDaysAsync(3);

        foreach (var todo in upcomingTodos)
        {
            var user = await _userRepository.GetByIdAsync(todo.CreatedById);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                string msg = $"Reminder: Your todo '{todo.Title}' is due on {todo.DueDate:yyyy-MM-dd}";
                await _mailService.SendEmail(user.Email, user.Username, msg);
            }
        }
    }
}
