using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SchoolTodoApi.Models
{
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Grade { get; set; } = null!;
       
     
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public string? SchoolId { get; set; } = ObjectId.GenerateNewId().ToString();

        
    }
}