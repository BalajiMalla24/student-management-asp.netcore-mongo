using MongoDB.Driver;
using SchoolTodoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolTodoApi.Services
{
    public class TodoItemService
    {
        private readonly IMongoCollection<TodoItem> _todoItems;

        public TodoItemService(ISchoolDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _todoItems = database.GetCollection<TodoItem>(settings.TodoItemsCollectionName);
        }

        public async Task<List<TodoItem>> GetAsync() =>
            await _todoItems.Find(item => true).ToListAsync();

        public async Task<TodoItem> GetAsync(string id) =>
            await _todoItems.Find(item => item.Id == id).FirstOrDefaultAsync();

        public async Task<List<TodoItem>> GetByEntityAsync(string entityId, string entityType) =>
            await _todoItems.Find(item => item.RelatedEntityId == entityId && item.RelatedEntityType == entityType).ToListAsync();

        public async Task<TodoItem> CreateAsync(TodoItem todoItem)
        {
            await _todoItems.InsertOneAsync(todoItem);
            return todoItem;
        }

        public async Task UpdateAsync(string id, TodoItem todoItemIn) =>
            await _todoItems.ReplaceOneAsync(item => item.Id == id, todoItemIn);

        public async Task RemoveAsync(string id) =>
            await _todoItems.DeleteOneAsync(item => item.Id == id);
    }
}