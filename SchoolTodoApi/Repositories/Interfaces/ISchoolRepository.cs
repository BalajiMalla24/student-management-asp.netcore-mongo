using SchoolTodoApi.Models;
using System.Linq.Expressions;

namespace SchoolTodoApi.Repositories.Interfaces
{
    public interface ISchoolRepository
    {
        // Base repository methods
        Task<School> GetByIdAsync(string id);
        Task<IEnumerable<School>> GetAllAsync();
        Task<IEnumerable<School>> FindAsync(Expression<Func<School, bool>> predicate);
        Task<School> FindOneAsync(Expression<Func<School, bool>> predicate);
        Task<School> CreateAsync(School entity);
        Task UpdateAsync(string id, School entity);
        Task DeleteAsync(string id);
        Task DeleteManyAsync(Expression<Func<School, bool>> predicate);

        // Custom school-specific method
        Task<School> GetSchoolWithStudentsAsync(string schoolId);
    }
}