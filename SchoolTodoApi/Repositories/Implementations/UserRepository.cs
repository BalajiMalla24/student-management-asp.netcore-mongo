using MongoDB.Driver;
using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;

namespace SchoolTodoApi.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ISchoolDatabaseSettings settings) : base(GetUserCollection(settings))
        {
        }

        private static IMongoCollection<User> GetUserCollection(ISchoolDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            return database.GetCollection<User>(settings.UsersCollectionName);
        }

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