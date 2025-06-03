using SchoolTodoApi.Models;
using System.Linq.Expressions;

namespace SchoolTodoApi.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        // Base repository methods
        Task<Student> GetByIdAsync(string id);
        Task<IEnumerable<Student>> GetAllAsync();
        Task<IEnumerable<Student>> FindAsync(Expression<Func<Student, bool>> predicate);
        Task<Student> FindOneAsync(Expression<Func<Student, bool>> predicate);
        Task<Student> CreateAsync(Student entity);
        Task UpdateAsync(string id, Student entity);
        Task DeleteAsync(string id);
        Task DeleteManyAsync(Expression<Func<Student, bool>> predicate);

        // Custom student-specific methods
        Task<List<StudentWithSchoolNameDto>> GetWithSchoolNamesAsync();
        Task<List<Student>> GetStudentsBySchoolIdAsync(string schoolId);
    }
}