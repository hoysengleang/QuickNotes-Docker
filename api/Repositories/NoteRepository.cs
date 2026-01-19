using Dapper;
using System.Data;
using api.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using api.Share;

namespace api.Repositories {
    public class NoteRepository : INoteRepository {
        private readonly IDbConnection _db;
        private readonly IDistributedCache _cache;

        public NoteRepository(IDbConnection db, IDistributedCache cache) {
            _db = db;
            _cache = cache;
        }

        public async Task<IEnumerable<Note>> GetAllAsync(int userId) {
            await CleanupTrashAsync();

            string key = $"notes_{userId}";
            
            var cached = await _cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cached)) {
                return JsonSerializer.Deserialize<IEnumerable<Note>>(cached);
            }

            var sql = "SELECT * FROM Notes WHERE UserId = @UserId AND IsDeleted = 0 ORDER BY CreatedAt DESC";
            var notes = await _db.QueryAsync<Note>(sql, new { UserId = userId });

            //  Redis (5 mins)
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(notes), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
            
            return notes;
        }

        public async Task<IEnumerable<Note>> GetTrashAsync(int userId) {
            // Cleanup old trash
            await CleanupTrashAsync();
            
            var sql = "SELECT * FROM Notes WHERE UserId = @UserId AND IsDeleted = 1 ORDER BY DeletedAt DESC";
            return await _db.QueryAsync<Note>(sql, new { UserId = userId });
        }

        public async Task<Note?> GetByIdAsync(int id, int userId) {
            var sql = "SELECT * FROM Notes WHERE Id = @Id AND UserId = @UserId";
            return await _db.QuerySingleOrDefaultAsync<Note>(sql, new { Id = id, UserId = userId });
        }

        public async Task<int> CreateAsync(Note note) {
            var sql = @"
                INSERT INTO Notes (Title, Content, UserId, CreatedAt, UpdatedAt, Color, IsPinned, ImageUrl, IsDeleted) 
                VALUES (@Title, @Content, @UserId, GETDATE(), GETDATE(), @Color, @IsPinned, @ImageUrl, 0);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            
            var parameters = new {
                note.Title,
                note.Content,
                note.UserId,
                note.Color,
                note.IsPinned,
                note.ImageUrl
            };

            var id = await _db.QuerySingleAsync<int>(sql, parameters);
            await _cache.RemoveAsync($"notes_{note.UserId}"); 
            return id;
        }

        public async Task UpdateAsync(Note note) {
            var sql = @"
                UPDATE Notes 
                SET Title = @Title, Content = @Content, Color = @Color, IsPinned = @IsPinned, ImageUrl = @ImageUrl, UpdatedAt = GETDATE()
                WHERE Id = @Id AND UserId = @UserId";
            
            var parameters = new {
                note.Id,
                note.UserId,
                note.Title,
                note.Content,
                note.Color,
                note.IsPinned,
                note.ImageUrl
            };

            await _db.ExecuteAsync(sql, parameters);
            await _cache.RemoveAsync($"notes_{note.UserId}");
        }

        // Soft Delete
        public async Task DeleteAsync(int id, int userId) {
            var sql = "UPDATE Notes SET IsDeleted = 1, DeletedAt = GETDATE() WHERE Id = @Id AND UserId = @UserId";
            await _db.ExecuteAsync(sql, new { Id = id, UserId = userId });
            await _cache.RemoveAsync($"notes_{userId}");
        }

        // Restore
        public async Task RestoreAsync(int id, int userId) {
            var sql = "UPDATE Notes SET IsDeleted = 0, DeletedAt = NULL WHERE Id = @Id AND UserId = @UserId";
            await _db.ExecuteAsync(sql, new { Id = id, UserId = userId });
            await _cache.RemoveAsync($"notes_{userId}");
        }

        // Hard Delete (Permanent)
        public async Task HardDeleteAsync(int id, int userId) {
            var sql = "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId";
            await _db.ExecuteAsync(sql, new { Id = id, UserId = userId });
            await _cache.RemoveAsync($"notes_{userId}");
        }

        private async Task CleanupTrashAsync() {
            var sql = "DELETE FROM Notes WHERE IsDeleted = 1 AND DeletedAt < DATEADD(day, -7, GETDATE())";
            await _db.ExecuteAsync(sql);
        }


        public async Task<PagedResult<Note>> GetAllAsync(int userId, int page, int pageSize)
        {
            await CleanupTrashAsync(); 

            string key = $"notes_{userId}_{page}_{pageSize}";

            var cached = await _cache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<PagedResult<Note>>(cached);
            }

            var sql = @"
                SELECT COUNT(*) FROM Notes WHERE UserId = @UserId AND IsDeleted = 0;

                SELECT * FROM Notes 
                WHERE UserId = @UserId AND IsDeleted = 0 
                ORDER BY CreatedAt DESC
                OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

            var parameters = new { 
                UserId = userId, 
                Skip = (page - 1) * pageSize, 
                Take = pageSize 
            };

            using (var multi = await _db.QueryMultipleAsync(sql, parameters))
            {
                var totalCount = await multi.ReadFirstAsync<int>();

                var notes = await multi.ReadAsync<Note>();

                var result = PageMeta.Paginate(notes, totalCount, page, pageSize);

                await _cache.SetStringAsync(
                    key, 
                    JsonSerializer.Serialize(result), 
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) }
                );

                return result;
            }
        }
    }
}