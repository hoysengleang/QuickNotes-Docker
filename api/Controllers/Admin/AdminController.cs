using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;

namespace api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IDbConnection _db;

        public AdminController(IDbConnection db)
        {
            _db = db;
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetErrorLogs([FromHeader(Name = "x-user-id")] int userId)
        {
            // 🔒 SECURITY: Verify Admin Status
            var isAdmin = await _db.ExecuteScalarAsync<bool>(
                "SELECT IsAdmin FROM Users WHERE Id = @Id", 
                new { Id = userId }
            );

            if (!isAdmin)
            {
                return StatusCode(403, new { message = "⛔ Access Denied. Admins only." });
            }

            // ✅ Fetch Logs
            var logs = await _db.QueryAsync("SELECT TOP 50 * FROM ErrorLogs ORDER BY CreatedAt DESC");
            return Ok(logs);
        }
    }
}