using Dapper;
using System.Data;
using api.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace api.Repositories {
    public class NoteRepository {
        private readonly IDbConnection _db;
        private readonly IDistributedCache _cache;

        public NoteRepository(IDbConnection db, IDistributedCache cache) {
            _db = db;
            _cache = cache;
        }

        public async Task<IEnumerable<Note>> GetAllAsync(int userId) {
            string key = $"notes_{userId}";
            
            // 1. Redis Cache Check
            var cached = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cached)) {
                return JsonSerializer.Deserialize<IEnumerable<Note>>(cached);
            }

            // 2. Database Fetch
            var sql = "SELECT * FROM Notes WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            var notes = await _db.QueryAsync<Note>(sql, new { UserId = userId });

            // 3. Save to Redis (5 mins)
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(notes), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
            
            return notes;
        }

        public async Task CreateAsync(Note note) {
            var sql = "INSERT INTO Notes (Title, Content, UserId, CreatedAt) VALUES (@Title, @Content, @UserId, GETDATE())";
            await _db.ExecuteAsync(sql, note);
            await _cache.RemoveAsync($"notes_{note.UserId}"); 
        }
    }
}