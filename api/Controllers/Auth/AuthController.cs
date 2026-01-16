using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Repositories;

namespace api.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Username and Password are required" });

            if (await _repo.UserExistsAsync(request.Username))
                return BadRequest(new { message = "Username already exists." });

            string passwordHash = ComputeSha256Hash(request.Password);

            var newUser = new User {
                Username = request.Username,
                PasswordHash = passwordHash,
                IsAdmin = false
            };

            var newId = await _repo.CreateUserAsync(newUser);

            return Ok(new { Id = newId, request.Username, IsAdmin = false });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Username and Password are required" });

            var user = await _repo.GetUserByUsernameAsync(request.Username);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password." });

            string inputHash = ComputeSha256Hash(request.Password);
            if (user.PasswordHash != inputHash)
                 return Unauthorized(new { message = "Invalid username or password." });

            return Ok(new { user.Id, user.Username, user.IsAdmin });
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}