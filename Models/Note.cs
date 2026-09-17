namespace backend_api.Models
{
    /// <summary>
    /// Represents a note domain model mapped to the "Notes" table in SQL Server.
    /// Each note belongs to a specific user identified by <see cref="UserId"/>.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Unique primary key identifier for the note.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key referencing the <see cref="User.Id"/> who owns this note.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Title or summary of the note.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Optional markdown or rich text body content of the note.
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// UTC timestamp when this note was initially created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// UTC timestamp when this note was last modified.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
