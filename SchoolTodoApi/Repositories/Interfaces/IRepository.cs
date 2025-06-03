using System.Linq.Expressions;

namespace SchoolTodoApi.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindOneAsync(Expression<Func<T, bool>> predicate);
        Task<T> CreateAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
        Task DeleteManyAsync(Expression<Func<T, bool>> predicate);
    }
}