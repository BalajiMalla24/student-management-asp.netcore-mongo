using MongoDB.Driver;
using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SchoolTodoApi.Repositories.Implementations
{
    public class TodoItemRepository : ITodoItemRepository
    {
        private readonly IMongoCollection<TodoItem> _collection;

        public TodoItemRepository(ISchoolDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<TodoItem>(settings.TodoItemsCollectionName);
        }

        // Base repository methods migrated from Repository<T>
        public async Task<TodoItem> GetByIdAsync(string id)
        {
            return await _collection.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<TodoItem>> FindAsync(Expression<Func<TodoItem, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<TodoItem> FindOneAsync(Expression<Func<TodoItem, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<TodoItem> CreateAsync(TodoItem entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

public async Task UpdateAsync(string id, TodoItem entity)
{
    entity.Id = id; // Ensure _id matches
    var result = await _collection.ReplaceOneAsync(t => t.Id == id, entity);

    if (result.MatchedCount == 0)
        throw new KeyNotFoundException($"TodoItem with id {id} not found.");
}
        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(t => t.Id == id);
        }

        public async Task DeleteManyAsync(Expression<Func<TodoItem, bool>> predicate)
        {
            await _collection.DeleteManyAsync(predicate);
        }

        // Existing custom methods
        public async Task<List<TodoItem>> GetByEntityAsync(string entityId, string entityType)
        {
            return await _collection.Find(item => item.RelatedEntityId == entityId && item.RelatedEntityType == entityType).ToListAsync();
        }

        public async Task<List<TodoItem>> GetByCreatedByIdAsync(string createdById)
        {
            return await _collection.Find(item => item.CreatedById == createdById).ToListAsync();
        }

        public async Task<List<TodoItem>> GetTodosDueInNextDaysAsync(int days)
        {
            var now = DateTime.UtcNow;
            var future = now.AddDays(days);

            return await _collection.Find(todo =>
                todo.DueDate >= now &&
                todo.DueDate <= future &&
                !todo.IsCompleted).ToListAsync();
        }
    }
}