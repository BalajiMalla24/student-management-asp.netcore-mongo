// Models/StudentWithSchoolNameDto.cs
namespace SchoolTodoApi.Models
{
    public class StudentWithSchoolNameDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Grade { get; set; } = null!;
        public string? SchoolId { get; set; }
        public string? SchoolName { get; set; }
    }
}
