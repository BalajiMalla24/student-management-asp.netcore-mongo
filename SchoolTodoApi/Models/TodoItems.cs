using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SchoolTodoApi.Models
{
    public class TodoItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime DueDate { get; set; }

        public string RelatedEntityType { get; set; } = string.Empty; // "Student" or "School"

        public string? DocumentUrl { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string RelatedEntityId { get; set; } = string.Empty;

        // NEW: ID of the user who created this todo
        [BsonRepresentation(BsonType.ObjectId)]
        public string CreatedById { get; set; } = string.Empty;
    }
}
