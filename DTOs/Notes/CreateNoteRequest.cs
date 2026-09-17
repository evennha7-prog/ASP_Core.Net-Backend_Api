using System.ComponentModel.DataAnnotations;

namespace backend_api.DTOs.Notes
{
    /// <summary>
    /// Request payload for creating a new note.
    /// </summary>
    public class CreateNoteRequest
    {
        /// <summary>
        /// Mandatory title of the note (between 1 and 255 characters).
        /// </summary>
        [Required(ErrorMessage = "Title is mandatory")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 255 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Optional body content of the note (supports markdown and plain text).
        /// </summary>
        public string? Content { get; set; }
    }
}
