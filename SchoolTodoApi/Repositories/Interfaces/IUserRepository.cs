using System.Linq.Expressions;
using SchoolTodoApi.Models;

namespace SchoolTodoApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // Generic repository methods
        Task<User> GetByIdAsync(string id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate);
        Task<User> FindOneAsync(Expression<Func<User, bool>> predicate);
        Task<User> CreateAsync(User entity);
        Task UpdateAsync(string id, User entity);
        Task DeleteAsync(string id);
        Task DeleteManyAsync(Expression<Func<User, bool>> predicate);
        
        // User-specific methods
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsEmailExistsAsync(string email);
    }
}