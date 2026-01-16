using api.Models;

namespace api.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> UserExistsAsync(string username);
        Task<int> CreateUserAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
