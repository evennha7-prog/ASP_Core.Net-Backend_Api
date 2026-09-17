using backend_api.DTOs.Notes;

namespace backend_api.Services.Interfaces
{
    /// <summary>
    /// Service contract providing business operations for managing notes.
    /// Ensures note access is scoped to the requesting user.
    /// </summary>
    public interface INoteService
    {
        /// <summary>
        /// Gets all notes for a specific user with optional search and sorting.
        /// </summary>
        /// <param name="userId">ID of the authenticated user.</param>
        /// <param name="search">Optional search term.</param>
        /// <param name="sort">Optional sort direction/column.</param>
        /// <returns>Collection of note response DTOs.</returns>
        Task<IEnumerable<NoteResponse>> GetAllNotesAsync(int userId, string? search = null, string? sort = null);

        /// <summary>
        /// Gets a note by ID for a specific user.
        /// </summary>
        /// <param name="id">Note ID.</param>
        /// <param name="userId">Authenticated user ID.</param>
        /// <returns>The note DTO if found; otherwise, null.</returns>
        Task<NoteResponse?> GetNoteByIdAsync(int id, int userId);

        /// <summary>
        /// Creates a new note for the authenticated user.
        /// </summary>
        /// <param name="userId">Authenticated user ID.</param>
        /// <param name="request">Note title and content payload.</param>
        /// <returns>The created note response DTO with generated ID and timestamps.</returns>
        Task<NoteResponse> CreateNoteAsync(int userId, CreateNoteRequest request);

        /// <summary>
        /// Updates an existing note owned by the authenticated user.
        /// </summary>
        /// <param name="id">Note ID.</param>
        /// <param name="userId">Authenticated user ID.</param>
        /// <param name="request">Updated note data.</param>
        /// <returns>The updated note DTO if found; otherwise, null.</returns>
        Task<NoteResponse?> UpdateNoteAsync(int id, int userId, UpdateNoteRequest request);

        /// <summary>
        /// Deletes a note owned by the authenticated user.
        /// </summary>
        /// <param name="id">Note ID.</param>
        /// <param name="userId">Authenticated user ID.</param>
        /// <returns>True if successfully deleted; otherwise, false.</returns>
        Task<bool> DeleteNoteAsync(int id, int userId);
    }
}
