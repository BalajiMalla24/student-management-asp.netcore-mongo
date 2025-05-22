using MongoDB.Driver;
using SchoolTodoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolTodoApi.Services
{
    public class SchoolService
    {
        private readonly IMongoCollection<School> _schools;
        private readonly IMongoCollection<Student> _students;

        public SchoolService(ISchoolDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _schools = database.GetCollection<School>(settings.SchoolsCollectionName);
            _students = database.GetCollection<Student>(settings.StudentsCollectionName);
        }

        public async Task<List<School>> GetAsync() =>
            await _schools.Find(school => true).ToListAsync();

        public async Task<School> GetAsync(string id) =>
            await _schools.Find(school => school.Id == id).FirstOrDefaultAsync();

        public async Task<School> CreateAsync(School school)
        {
            await _schools.InsertOneAsync(school);
            return school;
        }

        public async Task UpdateAsync(string id, School schoolIn) =>
            await _schools.ReplaceOneAsync(school => school.Id == id, schoolIn);

        public async Task RemoveAsync(string id)
        {
            var school = await GetAsync(id);

            if (school != null && school.StudentIds.Any())
            {
                var filter = Builders<Student>.Filter.In(s => s.Id, school.StudentIds);
                await _students.DeleteManyAsync(filter);
            }

            await _schools.DeleteOneAsync(s => s.Id == id);
        }
    }
}
