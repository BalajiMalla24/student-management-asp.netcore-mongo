using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchoolTodoApi.Models;
using SchoolTodoApi.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolTodoApi.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<string> Register(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Email is required");
            }

            // Check if username or email already exists
            var existingUsername = await _userRepository.IsUsernameExistsAsync(user.Username);
            if (existingUsername)
            {
                throw new ArgumentException("Username already exists");
            }

            var existingEmail = await _userRepository.IsEmailExistsAsync(user.Email);
            if (existingEmail)
            {
                throw new ArgumentException("Email already exists");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            await _userRepository.CreateAsync(user);
            return user.Id;
        }

        public async Task<(string? Token, string? Role, string? id)> Login(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return (null, null, null);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id),
                new Claim("email", user.Email ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenValidityMins),
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return (jwtToken, user.Role, user.Id);
        }
    }
}