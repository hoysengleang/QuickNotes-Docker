using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using api.Models;

namespace api.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDbConnection _db;

        public AuthController(IDbConnection db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (string.IsNullOrEmpty(request.Username))
                return BadRequest(new { message = "Username is required" });

            // 1. Check if user exists
            var sqlUser = "SELECT * FROM Users WHERE Username = @Username";
            var user = await _db.QuerySingleOrDefaultAsync<User>(sqlUser, new { request.Username });

            if (user == null)
            {
                // 2. If not, REGISTER them automatically (Simulating a quick onboarding)
                var insertSql = @"
                    INSERT INTO Users (Username, IsAdmin) 
                    VALUES (@Username, 0); 
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                
                var newId = await _db.ExecuteScalarAsync<int>(insertSql, new { request.Username });
                
                user = new User { Id = newId, Username = request.Username, IsAdmin = false };
            }

            // 3. Return the user info (Frontend will save this to localStorage)
            return Ok(user);
        }
    }
}