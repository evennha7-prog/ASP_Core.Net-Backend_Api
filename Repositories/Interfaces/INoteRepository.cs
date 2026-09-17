using backend_api.Models;

namespace backend_api.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface abstracting direct database operations on the "Notes" table.
    /// Ensures all note interactions are scoped to the authenticated user.
    /// </summary>
    public interface INoteRepository
    {
        /// <summary>
        /// Retrieves all notes belonging to a specific user, with optional search query and sorting.
        /// </summary>
        /// <param name="userId">The ID of the user owning the notes.</param>
        /// <param name="search">Optional search keyword to match against Title or Content.</param>
        /// <param name="sort">Optional sort order ('oldest', 'title_asc', 'title_desc', 'updated', or default newest).</param>
        /// <returns>Collection of notes matching criteria.</returns>
        Task<IEnumerable<Note>> GetAllByUserIdAsync(int userId, string? search = null, string? sort = null);

        /// <summary>
        /// Retrieves a single note by its ID and user ID to prevent unauthorized access.
        /// </summary>
        /// <param name="id">Note primary key ID.</param>
        /// <param name="userId">User ID who must own the note.</param>
        /// <returns>The Note entity if found and owned by user; otherwise, null.</returns>
        Task<Note?> GetByIdAsync(int id, int userId);

        /// <summary>
        /// Inserts a new note record into the database.
        /// </summary>
        /// <param name="note">Note model to insert.</param>
        /// <returns>The generated ID of the created note.</returns>
        Task<int> CreateAsync(Note note);

        /// <summary>
        /// Updates an existing note's title, content, and updated timestamp.
        /// </summary>
        /// <param name="note">Note model containing updated data.</param>
        /// <returns>True if the note was found and updated; otherwise, false.</returns>
        Task<bool> UpdateAsync(Note note);

        /// <summary>
        /// Deletes a note permanently from the database.
        /// </summary>
        /// <param name="id">Note ID to delete.</param>
        /// <param name="userId">User ID owning the note.</param>
        /// <returns>True if a row was deleted; otherwise, false.</returns>
        Task<bool> DeleteAsync(int id, int userId);
    }
}
