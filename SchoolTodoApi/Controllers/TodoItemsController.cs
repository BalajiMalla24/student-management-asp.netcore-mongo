using Microsoft.AspNetCore.Mvc;
using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;
using SchoolTodoApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace SchoolTodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
         private readonly TodoItemService _todoItemService;
        private readonly ITodoItemRepository _todoItemRepository;
        private readonly IUserRepository _userRepository;
        private readonly S3Service _s3service;
        private readonly MailService _mailservice;
          private readonly IMongoCollection<User> _users;

        public TodoItemsController(
            TodoItemService todoItemService ,
            ITodoItemRepository todoItemRepository,
            IUserRepository userRepository,
            S3Service s3service,
            MailService mailservice,
            ISchoolDatabaseSettings settings
            )
        {
            _todoItemService = todoItemService;
            _todoItemRepository = todoItemRepository;
            _userRepository = userRepository;
            _s3service = s3service;
            _mailservice = mailservice;


            var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _users = database.GetCollection<User>("Users"); 
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> Get()
        {
            var items = await _todoItemRepository.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}", Name = "GetTodoItem")]
        public async Task<ActionResult<TodoItem>> Get(string id)
        {
            var item = await _todoItemRepository.GetByIdAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [HttpGet("entity/{entityId}/{entityType}")]
        public async Task<ActionResult<List<TodoItem>>> GetByEntity(string entityId, string entityType)
        {
            var items = await _todoItemRepository.GetByEntityAsync(entityId, entityType);
            return Ok(items);
        }

       [HttpPost]
public async Task<ActionResult<object>> Create([FromForm] TodoItem todoItem, [FromForm] IFormFile? document)
{
    if (document != null)
    {
        var s3url = await _s3service.UploadFileAsync(document);
        if (string.IsNullOrEmpty(s3url))
        {
            return BadRequest("File upload failed");
        }
        todoItem.DocumentUrl = s3url;
    }

    // Get user details by CreatedById (instead of RelatedEntityId)
    var user = await _users.Find(u => u.Id == todoItem.CreatedById).FirstOrDefaultAsync();

    bool emailSent = false;
    string emailStatusMessage = "";
    string email = "";

    if (user != null && !string.IsNullOrEmpty(user.Email))
    {
        string username = user.Username;
        string emailMessage = $"Hello {user.Username},\n\nYour new todo '{todoItem.Title}' has been created successfully.";

        try
        {
            await _mailservice.SendEmail(user.Email, username, emailMessage);
            email = user.Email;
            emailSent = true;
            emailStatusMessage = "Email notification sent successfully.";
        }
        catch (Exception ex)
        {
            emailStatusMessage = $"Todo created, but failed to send email: {ex.Message}";
        }
    }

    await _todoItemService.CreateAsync(todoItem);

    return CreatedAtRoute("GetTodoItem", new { id = todoItem.Id }, new
    {
        todoItem,
        emailStatus = emailStatusMessage,
        emailSent,
        email
    });
}


      [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] TodoItem todoItemIn, IFormFile? document)
        {
            var existing = await _todoItemRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            todoItemIn.DocumentUrl = document != null
                ? await _s3service.UploadFileAsync(document)
                : existing.DocumentUrl;

            todoItemIn.CreatedById = existing.CreatedById; // retain original CreatedById
            todoItemIn.Id = id;

            await _todoItemRepository.UpdateAsync(id, todoItemIn);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _todoItemRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _todoItemRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
