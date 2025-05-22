using Microsoft.AspNetCore.Mvc;
using SchoolTodoApi.Models;
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
        private readonly S3Service _s3service;
        private readonly MailService _mailservice;
        private readonly IMongoCollection<User> _users;
        public TodoItemsController(TodoItemService todoItemService , S3Service s3service , MailService mailservice , ISchoolDatabaseSettings settings)
        {
            _todoItemService = todoItemService;
            _s3service = s3service;
            _mailservice = mailservice;


        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _users = database.GetCollection<User>("Users"); 
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> Get() =>
            await _todoItemService.GetAsync();

        [HttpGet("{id}", Name = "GetTodoItem")]
        public async Task<ActionResult<TodoItem>> Get(string id)
        {
            var todoItem = await _todoItemService.GetAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        [HttpGet("entity/{entityId}/{entityType}")]
        public async Task<ActionResult<List<TodoItem>>> GetByEntity(string entityId, string entityType) =>
            await _todoItemService.GetByEntityAsync(entityId, entityType);
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
        public async Task<IActionResult> Update(string id, [FromForm] TodoItem todoItemIn , IFormFile? document)
        {
            var existing = await _todoItemService.GetAsync(id);

            if (existing == null)
            {
                return NotFound();
            }
          
           if(document != null){
             todoItemIn.DocumentUrl = await _s3service.UploadFileAsync(document);
          }
            else{
                todoItemIn.DocumentUrl = existing.DocumentUrl;
            }
          
             todoItemIn.Id =id;
            await _todoItemService.UpdateAsync(id, todoItemIn);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var todoItem = await _todoItemService.GetAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            await _todoItemService.RemoveAsync(id);

            return NoContent();
        }

        
    }
}