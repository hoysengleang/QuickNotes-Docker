using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using api.Controllers;
using api.Models;
using api.Repositories;

namespace api.tests
{
    public class NotesControllerTests
    {
        private readonly Mock<INoteRepository> _repoMock;
        private readonly NotesController _controller;

        public NotesControllerTests()
        {
            _repoMock = new Mock<INoteRepository>();
            _controller = new NotesController(_repoMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotes()
        {
            var notes = new List<Note> { new Note { Id = 1, Title = "Note 1" } };
            _repoMock.Setup(r => r.GetAllAsync(1)).ReturnsAsync(notes);

            var result = await _controller.GetAll(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedNotes = Assert.IsAssignableFrom<IEnumerable<Note>>(okResult.Value);
            Assert.Single(returnedNotes);
        }

        [Fact]
        public async Task GetById_ShouldReturnNote_WhenFound()
        {
            var note = new Note { Id = 10, Title = "Found me" };
            _repoMock.Setup(r => r.GetByIdAsync(10, 1)).ReturnsAsync(note);

            var result = await _controller.GetById(10, 1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(note, okResult.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenNull()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Note?)null);

            var result = await _controller.GetById(99, 1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            var note = new Note { Title = "New Note", UserId = 1 };
            _repoMock.Setup(r => r.CreateAsync(note)).ReturnsAsync(100);

            var result = await _controller.Create(note);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(100, ((Note)createdResult.Value).Id);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            _repoMock.Setup(r => r.DeleteAsync(5, 1)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(5, 1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
