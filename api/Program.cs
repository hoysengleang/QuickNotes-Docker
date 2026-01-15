using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using api.Repositories;
using api.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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
builder.Services.AddScoped<NoteRepository>();

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
// ✅ DB Initialization Logic
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
            InitialCatalog = "master" // 👈 Switch to 'master' database
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
                if (retries > 30) throw; // Give up after 2.5 mins
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
                    IsAdmin BIT DEFAULT 0
                );

                IF OBJECT_ID('Notes', 'U') IS NULL 
                CREATE TABLE Notes (
                    Id INT IDENTITY(1,1) PRIMARY KEY, 
                    Title NVARCHAR(100), 
                    Content NVARCHAR(MAX), 
                    UserId INT, 
                    CreatedAt DATETIME DEFAULT GETDATE()
                );

                IF OBJECT_ID('ErrorLogs', 'U') IS NULL 
                CREATE TABLE ErrorLogs (
                    Id INT IDENTITY(1,1) PRIMARY KEY, 
                    Message NVARCHAR(MAX), 
                    StackTrace NVARCHAR(MAX), 
                    CreatedAt DATETIME DEFAULT GETDATE()
                );
            ";
            db.Execute(sql);

            // 6. Create Admin User
            var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM Users WHERE Username='Admin'");
            if (count == 0)
            {
                db.Execute("INSERT INTO Users (Username, IsAdmin) VALUES ('Admin', 1)");
                logger.LogInformation("👤 Admin user created.");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError("❌ Critical DB Init Error: " + ex.Message);
    }
}

// ✅ .NET 8 Swagger Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();