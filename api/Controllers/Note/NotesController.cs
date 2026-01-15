using Microsoft.AspNetCore.Mvc;
using api.Repositories;

namespace api.Controllers.Note
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NoteRepository _repo;

        public NotesController(NoteRepository repo)
        {
            _repo = repo;
        }

        // GET: api/notes
        [HttpGet]
        public async Task<IActionResult> GetNotes([FromHeader(Name = "x-user-id")] int userId)
        {
            if (userId <= 0) return Unauthorized("User ID header is missing.");

            var notes = await _repo.GetAllAsync(userId);
            return Ok(notes);
        }

        // POST: api/notes
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] api.Models.Note note, [FromHeader(Name = "x-user-id")] int userId)
        {
            if (userId <= 0) return Unauthorized("User ID header is missing.");

            // Force the Note to belong to the Header User (Security)
            note.UserId = userId;

            await _repo.CreateAsync(note);
            return Ok(new { message = "Note created successfully" });
        }
    }
}