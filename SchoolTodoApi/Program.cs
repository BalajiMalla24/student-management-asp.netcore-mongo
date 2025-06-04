using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using SchoolTodoApi.Models;
using SchoolTodoApi.Services;
using SchoolTodoApi.Repositories.Interfaces;
using SchoolTodoApi.Repositories.Implementations;
using System.Text;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;

var builder = WebApplication.CreateBuilder(args);

// ---------------------- MVC ----------------------
builder.Services.AddControllersWithViews();

// ---------------------- Configuration ----------------------
builder.Services.Configure<SchoolDatabaseSettings>(
    builder.Configuration.GetSection("SchoolDatabaseSettings"));
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<AwsSettings>(
    builder.Configuration.GetSection("AwsSettings"));
builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection("MailSettings"));

// Singleton for DB settings
builder.Services.AddSingleton<ISchoolDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<SchoolDatabaseSettings>>().Value);

// ---------------------- Repositories ----------------------
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ---------------------- Services ----------------------
builder.Services.AddScoped<TodoItemService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<S3Service>();
builder.Services.AddScoped<ReminderService>(); // Reminder job service

// ---------------------- JWT Authentication ----------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

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

builder.Services.AddAuthorization();

// ---------------------- CORS ----------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ---------------------- Hangfire Configuration ----------------------
var mongoConnection = builder.Configuration["SchoolDatabaseSettings:ConnectionString"];
var mongoDbName = builder.Configuration["SchoolDatabaseSettings:DatabaseName"];

var hangfireOptions = new MongoStorageOptions
{
    MigrationOptions = new MongoMigrationOptions
    {
        MigrationStrategy = new MigrateMongoMigrationStrategy(), // Automatically migrate schema
        BackupStrategy = new CollectionMongoBackupStrategy()     // Optional: backup collections
    }
};

builder.Services.AddHangfire(config =>
    config.UseMongoStorage(mongoConnection, mongoDbName, hangfireOptions));

builder.Services.AddHangfireServer();

// ---------------------- Build the app ----------------------
var app = builder.Build();

// ---------------------- Middleware ----------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

// ---------------------- Hangfire Dashboard + Job ----------------------
app.UseHangfireDashboard(); // Access at /hangfire

RecurringJob.AddOrUpdate<ReminderService>(
    "todo-reminder",
    service => service.SendUpcomingTodoReminders(),
    Cron.Daily
);

// ---------------------- Routing ----------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
