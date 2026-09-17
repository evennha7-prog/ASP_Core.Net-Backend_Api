namespace backend_api.DTOs.Notes
{
    /// <summary>
    /// Response model representing note data returned to the client.
    /// Excludes internal user identifiers for cleanliness and security.
    /// </summary>
    public class NoteResponse
    {
        /// <summary>
        /// Unique note identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Note headline or title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Note body content (markdown or plain text).
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// UTC timestamp when note was initially created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// UTC timestamp when note was last modified.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
