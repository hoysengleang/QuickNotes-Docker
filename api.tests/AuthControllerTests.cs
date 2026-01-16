using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using api.Controllers.Auth;
using api.Models;
using api.Repositories;

namespace api.tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthRepository> _repoMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _repoMock = new Mock<IAuthRepository>();
            _controller = new AuthController(_repoMock.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            var username = "testuser";
            var password = "password123";
            var hash = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f";

            var user = new User { Id = 1, Username = username, PasswordHash = hash, IsAdmin = false };

            _repoMock.Setup(r => r.GetUserByUsernameAsync(username)).ReturnsAsync(user);

            var loginDto = new LoginDto { Username = username, Password = password };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            _repoMock.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            var result = await _controller.Login(new LoginDto { Username = "unknown", Password = "pwd" });

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsNew()
        {
            _repoMock.Setup(r => r.UserExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repoMock.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(99);

            var result = await _controller.Register(new RegisterDto { Username = "newuser", Password = "pwd" });

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenUserExists()
        {
            _repoMock.Setup(r => r.UserExistsAsync("existing")).ReturnsAsync(true);

            var result = await _controller.Register(new RegisterDto { Username = "existing", Password = "pwd" });

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
