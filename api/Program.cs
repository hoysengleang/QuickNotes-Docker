using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using api.Repositories;
using api.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

// Database Connection (Scoped)
builder.Services.AddScoped<IDbConnection>(sp => 
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "Notes_";
});

// Repositories
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// ---------------------------
// DB Initialization Logic
// ---------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var configuration = services.GetRequiredService<IConfiguration>();

    try
    {
        // 1. Get the original connection string
        var originalConnectionString = configuration.GetConnectionString("DefaultConnection");

        // 2. Create a modified string pointing to 'master' so we can create the DB
        var masterConnectionString = new SqlConnectionStringBuilder(originalConnectionString)
        {
            InitialCatalog = "master" 
        }.ConnectionString;

        // 3. Retry Loop: Wait for SQL Server to wake up
        int retries = 0;
        while (true)
        {
            try
            {
                logger.LogInformation("⏳ Connecting to SQL Server (master)...");
                using (var masterDb = new SqlConnection(masterConnectionString))
                {
                    masterDb.Open();
                    
                    // 4. Create the Database if it doesn't exist
                    masterDb.Execute("IF DB_ID('NotesDb') IS NULL CREATE DATABASE NotesDb");
                    logger.LogInformation("✅ Database 'NotesDb' created or already exists.");
                }
                break; // Success! Exit loop.
            }
            catch (Exception ex)
            {
                retries++;
                if (retries > 30) throw; 
                logger.LogWarning($"⚠️ SQL Server not ready. Retrying in 5s... (Attempt {retries}/30) Error: {ex.Message}");
                System.Threading.Thread.Sleep(5000);
            }
        }

        // 5. NOW connect to the real 'NotesDb' and create tables
        using (var db = new SqlConnection(originalConnectionString))
        {
            db.Open();
            var sql = @"
                IF OBJECT_ID('Users', 'U') IS NULL 
                CREATE TABLE Users (
                    Id INT IDENTITY(1,1) PRIMARY KEY, 
                    Username NVARCHAR(50), 
                    PasswordHash NVARCHAR(255),
                    IsAdmin BIT DEFAULT 0
                );

                -- Migration: Add PasswordHash if it doesn't exist
                IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'PasswordHash' AND Object_ID = Object_ID(N'Users'))
                BEGIN
                    ALTER TABLE Users ADD PasswordHash NVARCHAR(255) DEFAULT '';
                END

                IF OBJECT_ID('Notes', 'U') IS NULL 
                CREATE TABLE Notes (
                    Id INT IDENTITY(1,1) PRIMARY KEY, 
                    Title NVARCHAR(100), 
                    Content NVARCHAR(MAX), 
                    UserId INT, 
                    CreatedAt DATETIME DEFAULT GETDATE(),
                    UpdatedAt DATETIME NULL,
                    Color NVARCHAR(20) DEFAULT 'default',
                    IsPinned BIT DEFAULT 0
                );

                -- Migration: Add UpdatedAt if it doesn't exist
                IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'UpdatedAt' AND Object_ID = Object_ID(N'Notes'))
                BEGIN
                    ALTER TABLE Notes ADD UpdatedAt DATETIME NULL;
                END

                -- Migration: Add Color and IsPinned
                IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'Color' AND Object_ID = Object_ID(N'Notes'))
                BEGIN
                    ALTER TABLE Notes ADD Color NVARCHAR(20) DEFAULT 'default';
                END

                IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'IsPinned' AND Object_ID = Object_ID(N'Notes'))
                BEGIN
                    ALTER TABLE Notes ADD IsPinned BIT DEFAULT 0;
                END

                IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ImageUrl' AND Object_ID = Object_ID(N'Notes'))
                BEGIN
                    ALTER TABLE Notes ADD ImageUrl NVARCHAR(MAX) NULL;
                END

                IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'IsDeleted' AND Object_ID = Object_ID(N'Notes'))
                BEGIN
                    ALTER TABLE Notes ADD IsDeleted BIT DEFAULT 0;
                END

                IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'DeletedAt' AND Object_ID = Object_ID(N'Notes'))
                BEGIN
                    ALTER TABLE Notes ADD DeletedAt DATETIME NULL;
                END

                IF OBJECT_ID('ErrorLogs', 'U') IS NULL 
                CREATE TABLE ErrorLogs (
                    Id INT IDENTITY(1,1) PRIMARY KEY, 
                    UserId INT NULL,
                    Message NVARCHAR(MAX) NOT NULL, 
                    StackTrace NVARCHAR(MAX) NULL, 
                    Source NVARCHAR(50) DEFAULT 'Unknown',
                    CreatedAt DATETIME DEFAULT GETDATE()
                );
            ";
            db.Execute(sql);

            // 6. Create Admin User
            var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM Users WHERE IsAdmin = 1");
            if (count == 0)
            {
                db.Execute("INSERT INTO Users (Username, IsAdmin) VALUES ('Admin', 1)");
                logger.LogInformation("👤 Admin user created.");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError("Critical DB Init Error: " + ex.Message);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();