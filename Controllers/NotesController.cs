using System.Security.Claims;
using backend_api.DTOs.Notes;
using backend_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_api.Controllers
{
    /// <summary>
    /// API Controller providing CRUD operations for notes.
    /// All endpoints require JWT Bearer authentication.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        /// <summary>
        /// Injects the note service.
        /// </summary>
        /// <param name="noteService">Business service for notes.</param>
        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        /// <summary>
        /// Helper method to extract the authenticated user's ID from Claims or HttpContext.Items.
        /// Throws an <see cref="UnauthorizedAccessException"/> if missing.
        /// </summary>
        private int GetCurrentUserId()
        {
            // First attempt to read NameIdentifier claim from ClaimsPrincipal
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            // Fallback to HttpContext.Items populated by JwtMiddleware
            if (HttpContext.Items.TryGetValue("UserId", out var itemUserId) && itemUserId is int id)
            {
                return id;
            }

            throw new UnauthorizedAccessException("User is not authenticated or valid user ID claim is missing.");
        }

        /// <summary>
        /// GET /api/notes?search=...&sort=...
        /// Retrieves all notes belonging to the logged-in user with optional search filtering and sorting.
        /// </summary>
        /// <param name="search">Optional search query to filter by title or content.</param>
        /// <param name="sort">Optional sorting order ('newest', 'oldest', 'title_asc', 'title_desc', 'updated').</param>
        /// <returns>HTTP 200 with list of notes.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NoteResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? sort)
        {
            var userId = GetCurrentUserId();
            var notes = await _noteService.GetAllNotesAsync(userId, search, sort);
            return Ok(notes);
        }

        /// <summary>
        /// GET /api/notes/{id}
        /// Retrieves a single note by ID for the logged-in user.
        /// </summary>
        /// <param name="id">Note ID.</param>
        /// <returns>HTTP 200 with NoteResponse, or HTTP 404 if not found.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();
            var note = await _noteService.GetNoteByIdAsync(id, userId);
            if (note == null)
            {
                return NotFound(new { message = $"Note with ID {id} not found." });
            }

            return Ok(note);
        }

        /// <summary>
        /// POST /api/notes
        /// Creates a new note for the logged-in user.
        /// </summary>
        /// <param name="request">Note title and content payload.</param>
        /// <returns>HTTP 201 Created with created note and Location header.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateNoteRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var createdNote = await _noteService.CreateNoteAsync(userId, request);
            return CreatedAtAction(nameof(GetById), new { id = createdNote.Id }, createdNote);
        }

        /// <summary>
        /// PUT /api/notes/{id}
        /// Updates an existing note owned by the logged-in user.
        /// </summary>
        /// <param name="id">Note ID.</param>
        /// <param name="request">Updated title and content payload.</param>
        /// <returns>HTTP 200 with updated note, or HTTP 404 if not found/unauthorized.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNoteRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var updatedNote = await _noteService.UpdateNoteAsync(id, userId, request);
            if (updatedNote == null)
            {
                return NotFound(new { message = $"Note with ID {id} not found or you do not have permission to edit it." });
            }

            return Ok(updatedNote);
        }

        /// <summary>
        /// DELETE /api/notes/{id}
        /// Deletes a note owned by the logged-in user.
        /// </summary>
        /// <param name="id">Note ID.</param>
        /// <returns>HTTP 204 No Content on success, or HTTP 404 if not found/unauthorized.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var deleted = await _noteService.DeleteNoteAsync(id, userId);
            if (!deleted)
            {
                return NotFound(new { message = $"Note with ID {id} not found or you do not have permission to delete it." });
            }

            return NoContent();
        }
    }
}
