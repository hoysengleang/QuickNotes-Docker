using api.Models;
using api.Share;

namespace api.Repositories
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllAsync(int userId);
        Task<IEnumerable<Note>> GetTrashAsync(int userId);
        Task<PagedResult<Note>> GetAllAsync(int userId, int page, int pageSize);
        Task<Note?> GetByIdAsync(int id, int userId);
        Task<int> CreateAsync(Note note);
        Task UpdateAsync(Note note);
        Task DeleteAsync(int id, int userId);
        Task RestoreAsync(int id, int userId);
        Task HardDeleteAsync(int id, int userId);
    }
}
