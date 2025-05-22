using Microsoft.AspNetCore.Http;
using System;

namespace SchoolTodoApi.Models
{
    public class TodoItemCreateDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DueDate { get; set; }
        public string RelatedEntityId { get; set; } = null!;
        public string RelatedEntityType { get; set; } = null!;
        public IFormFile? Document { get; set; }
    }
}
