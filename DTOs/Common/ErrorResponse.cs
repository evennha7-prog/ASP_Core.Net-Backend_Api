namespace backend_api.DTOs.Common
{
    /// <summary>
    /// Standardized error response contract returned by the global exception handler.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Always false for error payloads.
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// HTTP status code of the error.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Primary error message describing the cause of the failure.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional additional details or stack information in development mode.
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Optional collection of field-level validation errors.
        /// </summary>
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
