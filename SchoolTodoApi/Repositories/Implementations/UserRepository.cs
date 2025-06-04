using MongoDB.Driver;
using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SchoolTodoApi.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        protected readonly IMongoCollection<User> _collection;

        public UserRepository(ISchoolDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<User>(settings.UsersCollectionName);
        }

        // Generic repository methods implementation
        public async Task<User> GetByIdAsync(string id)
        {
           return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task<User> FindOneAsync(Expression<Func<User, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<User> CreateAsync(User entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(string id, User entity)
        {
            var filter = Builders<User>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task DeleteManyAsync(Expression<Func<User, bool>> predicate)
        {
            await _collection.DeleteManyAsync(predicate);
        }

        // User-specific methods implementation
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _collection.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            var count = await _collection.CountDocumentsAsync(u => u.Username == username);
            return count > 0;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            var count = await _collection.CountDocumentsAsync(u => u.Email == email);
            return count > 0;
        }
    }
}