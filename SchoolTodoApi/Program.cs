using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using SchoolTodoApi.Models;
using SchoolTodoApi.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);

// Register configuration for database settings
builder.Services.Configure<SchoolDatabaseSettings>(
    builder.Configuration.GetSection("SchoolDatabaseSettings"));
    builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AwsSettings"));

    builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddSingleton<ISchoolDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<SchoolDatabaseSettings>>().Value);

// jwt settings ka config
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();





// Register services
builder.Services.AddSingleton<StudentService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<StudentService>>();
    var settings = sp.GetRequiredService<ISchoolDatabaseSettings>();
    try
    {
        logger.LogInformation($"Connecting to MongoDB at {settings.ConnectionString}");
        return new StudentService(settings);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to connect to MongoDB");
        throw;
    }
});

builder.Services.AddSingleton<MailService>();
builder.Services.AddSingleton<S3Service>();
builder.Services.AddSingleton<SchoolService>();
builder.Services.AddSingleton<TodoItemService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddControllers();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

// // Configure Swagger
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "SchoolTodoApi",
//         Version = "v1",
//         Description = "A simple API for managing schools, students, and todo items"
//     });
// });

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// app.UseSwagger();
// app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();   
app.UseAuthorization();

app.MapControllers();

// Redirect root to Swagger UI
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.Run();
