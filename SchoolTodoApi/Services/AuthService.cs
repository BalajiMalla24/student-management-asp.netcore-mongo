using MongoDB.Driver;
using SchoolTodoApi.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

public class AuthService
{
    public readonly IMongoCollection<User> _users;
    private readonly JwtSettings _jwtSettings;

    public AuthService(ISchoolDatabaseSettings settings, IOptions<JwtSettings> jwtSettings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        _users = database.GetCollection<User>(settings.UsersCollectionName);
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<string> Register(User user, string password)
    {
        // Optional: Validate that email is provided
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException("Email is required");
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(password);
        await _users.InsertOneAsync(user);
        return user.Id;
    }

  public async Task<(string? Token, string? Role, string? id)> Login(string username, string password)
{
    var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
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
