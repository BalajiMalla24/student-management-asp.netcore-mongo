using MongoDB.Driver;
using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SchoolTodoApi.Repositories.Implementations
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly IMongoCollection<School> _collection;
        private readonly IMongoCollection<Student> _studentsCollection;

        public SchoolRepository(ISchoolDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<School>(settings.SchoolsCollectionName);
            _studentsCollection = database.GetCollection<Student>(settings.StudentsCollectionName);
        }

        // Base repository methods migrated from Repository<T>
        public async Task<School> GetByIdAsync(string id)
        {
            return await _collection.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<School>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<School>> FindAsync(Expression<Func<School, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<School> FindOneAsync(Expression<Func<School, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<School> CreateAsync(School entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(string id, School entity)
        {
            await _collection.ReplaceOneAsync(s => s.Id == id, entity);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(s => s.Id == id);
        }

        public async Task DeleteManyAsync(Expression<Func<School, bool>> predicate)
        {
            await _collection.DeleteManyAsync(predicate);
        }

        // Existing custom method
        public async Task<School> GetSchoolWithStudentsAsync(string schoolId)
        {
            var school = await GetByIdAsync(schoolId);
            if (school != null && school.StudentIds.Any())
            {
                var students = await _studentsCollection
                    .Find(s => school.StudentIds.Contains(s.Id))
                    .ToListAsync();
                // You can set students to school if needed
            }
            return school;
        }
    }
}