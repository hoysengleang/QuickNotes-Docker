using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Repositories;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _repo;

        public NotesController(INoteRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int userId, [FromQuery] int page, [FromQuery] int pageSize)
        {
            if (userId <= 0) return BadRequest("UserId is required.");
            var notes = await _repo.GetAllAsync(userId, page, pageSize);    
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] int userId)
        {
            var note = await _repo.GetByIdAsync(id, userId);
            if (note == null) return NotFound();
            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] api.Models.Note note)
        {
            if (string.IsNullOrEmpty(note.Title)) return BadRequest("Title is required.");
            
            var id = await _repo.CreateAsync(note);
            note.Id = id;
            return CreatedAtAction(nameof(GetById), new { id = note.Id, userId = note.UserId }, note);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] api.Models.Note note)
        {
            if (id != note.Id) return BadRequest("ID mismatch.");

            await _repo.UpdateAsync(note);
            return NoContent();
        }


        [HttpGet("trash")]
        public async Task<IActionResult> GetTrash([FromQuery] int userId)
        {
            if (userId <= 0) return BadRequest("UserId is required.");
            var notes = await _repo.GetTrashAsync(userId);
            return Ok(notes);
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id, [FromQuery] int userId)
        {
            await _repo.RestoreAsync(id, userId);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int userId, [FromQuery] bool permanent = false)
        {
            if (permanent) {
                await _repo.HardDeleteAsync(id, userId);
            } else {
                await _repo.DeleteAsync(id, userId);
            }
            return NoContent();
        }
    }
}
