using Microsoft.AspNetCore.Mvc;
using SchoolTodoApi.Models;

namespace SchoolTodoApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(UserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                return BadRequest("Email is required for registration.");
            }

            var user = new User
            {
                Username = userDto.Username,
                Role = userDto.Role,
                Email = userDto.Email
            };

            await _authService.Register(user, userDto.Password);
            return Ok("Registered successfully");
        }

        
     
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
{
    var (token, role, id ) = await _authService.Login(loginDto.Username, loginDto.Password);

    if (token == null )
    {
        return Unauthorized("Invalid credentials");
    }

    return Ok(new
    {
        token,
        role,
        id
    });
}


    }
}
