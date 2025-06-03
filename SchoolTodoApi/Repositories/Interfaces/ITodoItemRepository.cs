using SchoolTodoApi.Models;
using System.Linq.Expressions;

namespace SchoolTodoApi.Repositories.Interfaces
{
    public interface ITodoItemRepository
    {
        // Base repository methods
        Task<TodoItem> GetByIdAsync(string id);
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task<IEnumerable<TodoItem>> FindAsync(Expression<Func<TodoItem, bool>> predicate);
        Task<TodoItem> FindOneAsync(Expression<Func<TodoItem, bool>> predicate);
        Task<TodoItem> CreateAsync(TodoItem entity);
        Task UpdateAsync(string id, TodoItem entity);
        Task DeleteAsync(string id);
        Task DeleteManyAsync(Expression<Func<TodoItem, bool>> predicate);

        // Custom TodoItem-specific methods
        Task<List<TodoItem>> GetByEntityAsync(string entityId, string entityType);
        Task<List<TodoItem>> GetByCreatedByIdAsync(string createdById);
        Task<List<TodoItem>> GetTodosDueInNextDaysAsync(int days);
    }
}