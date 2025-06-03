using MongoDB.Driver;
using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SchoolTodoApi.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IMongoCollection<Student> _collection;
        private readonly IMongoCollection<School> _schoolsCollection;

        public StudentRepository(ISchoolDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<Student>(settings.StudentsCollectionName);
            _schoolsCollection = database.GetCollection<School>(settings.SchoolsCollectionName);
        }

        // Base repository methods migrated from Repository<T>
        public async Task<Student> GetByIdAsync(string id)
        {
            return await _collection.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Student>> FindAsync(Expression<Func<Student, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<Student> FindOneAsync(Expression<Func<Student, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<Student> CreateAsync(Student entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(string id, Student entity)
        {
            await _collection.ReplaceOneAsync(s => s.Id == id, entity);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(s => s.Id == id);
        }

        public async Task DeleteManyAsync(Expression<Func<Student, bool>> predicate)
        {
            await _collection.DeleteManyAsync(predicate);
        }

        // Existing custom methods
        public async Task<List<StudentWithSchoolNameDto>> GetWithSchoolNamesAsync()
        {
            var students = await _collection.Find(_ => true).ToListAsync();
            var schools = await _schoolsCollection.Find(_ => true).ToListAsync();

            var result = students.Select(s =>
            {
                var schoolName = schools.FirstOrDefault(sc => sc.Id == s.SchoolId)?.Name;
                return new StudentWithSchoolNameDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Age = s.Age,
                    Grade = s.Grade,
                    SchoolId = s.SchoolId,
                    SchoolName = schoolName
                };
            }).ToList();

            return result;
        }

        public async Task<List<Student>> GetStudentsBySchoolIdAsync(string schoolId)
        {
            return await _collection.Find(s => s.SchoolId == schoolId).ToListAsync();
        }
    }
}