using backend_api.DTOs.Notes;
using backend_api.Models;
using backend_api.Repositories.Interfaces;
using backend_api.Services.Interfaces;

namespace backend_api.Services
{
    /// <summary>
    /// Implements business logic for managing notes:
    /// Validates note content, coordinates database persistence through INoteRepository,
    /// and maps internal Note entities to client-facing NoteResponse DTOs.
    /// </summary>
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        /// <summary>
        /// Injects the note repository dependency.
        /// </summary>
        /// <param name="noteRepository">Data access repository for notes.</param>
        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        /// <summary>
        /// Retrieves all notes belonging to the specified user, mapped to response DTOs.
        /// </summary>
        public async Task<IEnumerable<NoteResponse>> GetAllNotesAsync(int userId, string? search = null, string? sort = null)
        {
            var notes = await _noteRepository.GetAllByUserIdAsync(userId, search, sort);
            return notes.Select(MapToResponse);
        }

        /// <summary>
        /// Retrieves a single note by ID, verifying user ownership.
        /// </summary>
        public async Task<NoteResponse?> GetNoteByIdAsync(int id, int userId)
        {
            var note = await _noteRepository.GetByIdAsync(id, userId);
            return note == null ? null : MapToResponse(note);
        }

        /// <summary>
        /// Creates a new note record associated with the given user.
        /// </summary>
        public async Task<NoteResponse> CreateNoteAsync(int userId, CreateNoteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Title is mandatory.", nameof(request.Title));
            }

            var now = DateTime.UtcNow;
            var note = new Note
            {
                UserId = userId,
                Title = request.Title.Trim(),
                Content = string.IsNullOrWhiteSpace(request.Content) ? null : request.Content.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            };

            var noteId = await _noteRepository.CreateAsync(note);
            note.Id = noteId;

            return MapToResponse(note);
        }

        /// <summary>
        /// Updates an existing note owned by the user. Returns null if note is not found or owned by another user.
        /// </summary>
        public async Task<NoteResponse?> UpdateNoteAsync(int id, int userId, UpdateNoteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Title is mandatory.", nameof(request.Title));
            }

            var existingNote = await _noteRepository.GetByIdAsync(id, userId);
            if (existingNote == null)
            {
                return null;
            }

            existingNote.Title = request.Title.Trim();
            existingNote.Content = string.IsNullOrWhiteSpace(request.Content) ? null : request.Content.Trim();
            existingNote.UpdatedAt = DateTime.UtcNow;

            var updated = await _noteRepository.UpdateAsync(existingNote);
            if (!updated)
            {
                return null;
            }

            return MapToResponse(existingNote);
        }

        /// <summary>
        /// Deletes a note belonging to the specified user.
        /// </summary>
        public async Task<bool> DeleteNoteAsync(int id, int userId)
        {
            return await _noteRepository.DeleteAsync(id, userId);
        }

        /// <summary>
        /// Helper mapping method that converts an internal domain <see cref="Note"/> entity
        /// to a public <see cref="NoteResponse"/> data transfer object.
        /// </summary>
        private static NoteResponse MapToResponse(Note note)
        {
            return new NoteResponse
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt
            };
        }
    }
}
