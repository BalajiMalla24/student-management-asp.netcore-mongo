namespace SchoolTodoApi.Models
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
