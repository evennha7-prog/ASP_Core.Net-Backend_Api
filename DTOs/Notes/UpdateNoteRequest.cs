using System.ComponentModel.DataAnnotations;

namespace backend_api.DTOs.Notes
{
    /// <summary>
    /// Request payload for updating an existing note's title and content.
    /// </summary>
    public class UpdateNoteRequest
    {
        /// <summary>
        /// Updated title of the note (mandatory, 1 to 255 characters).
        /// </summary>
        [Required(ErrorMessage = "Title is mandatory")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Updated body content of the note.
        /// </summary>
        public string? Content { get; set; }
    }
}
