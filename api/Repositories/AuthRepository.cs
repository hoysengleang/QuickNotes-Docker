using System.Data;
using Dapper;
using api.Models;

namespace api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnection _db;

        public AuthRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            var count = await _db.ExecuteScalarAsync<int>(sql, new { Username = username });
            return count > 0;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            var sql = @"
                INSERT INTO Users (Username, PasswordHash, IsAdmin) 
                VALUES (@Username, @PasswordHash, 0); 
                SELECT CAST(SCOPE_IDENTITY() as int);";
            
            return await _db.QuerySingleAsync<int>(sql, new { user.Username, user.PasswordHash });
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var sql = "SELECT * FROM Users WHERE Username = @Username";
            return await _db.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
        }
    }
}
