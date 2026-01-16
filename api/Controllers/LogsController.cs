using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using api.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IDbConnection _db;

        public LogsController(IDbConnection db)
        {
            _db = db;
        }

        // POST: api/logs
        [HttpPost]
        public async Task<IActionResult> CreateLog([FromBody] ErrorLog log)
        {
            var sql = @"
                INSERT INTO ErrorLogs (UserId, Message, StackTrace, Source, CreatedAt)
                VALUES (@UserId, @Message, @StackTrace, @Source, GETDATE());";
            
            await _db.ExecuteAsync(sql, log);
            return Ok();
        }

        // GET: api/logs?userId={userId}
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] int userId)
        {
            var adminCheckSql = "SELECT IsAdmin FROM Users WHERE Id = @Id";
            var isAdmin = await _db.ExecuteScalarAsync<bool>(adminCheckSql, new { Id = userId });

            if (!isAdmin)
            {
                return Unauthorized(new { message = "Access denied. Admin only." });
            }

            var sql = "SELECT * FROM ErrorLogs ORDER BY CreatedAt DESC";
            var logs = await _db.QueryAsync<ErrorLog>(sql);

            return Ok(logs);
        }
    }
}
