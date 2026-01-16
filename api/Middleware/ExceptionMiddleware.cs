using Dapper;
using System.Data;
using System.Net;
using System.Text.Json;

namespace api.Middleware {
    public class ExceptionMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IServiceProvider serviceProvider) {
            _next = next;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context) {
            try {
                await _next(context);
            } catch (Exception ex) {
                _logger.LogError(ex, "❌ SERVER ERROR");
                await LogToDatabase(ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task LogToDatabase(Exception ex) {
            using (var scope = _serviceProvider.CreateScope()) {
                try {
                    var db = scope.ServiceProvider.GetRequiredService<IDbConnection>();
                    var sql = "INSERT INTO ErrorLogs (Message, StackTrace, CreatedAt) VALUES (@Message, @StackTrace, GETDATE())";
                    await db.ExecuteAsync(sql, new { Message = ex.Message, StackTrace = ex.ToString() });
                } catch { 
                    /* If DB is down, just suppress */ 
                }
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception) {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new { Message = "Internal Server Error" }));
        }
    }
}