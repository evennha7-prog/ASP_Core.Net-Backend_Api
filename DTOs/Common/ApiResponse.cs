namespace backend_api.DTOs.Common
{
    /// <summary>
    /// Standardized API response envelope for uniform response formatting across all endpoints.
    /// </summary>
    /// <typeparam name="T">Payload data type.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the API call completed successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Optional human-readable message or notification.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Payload data returned by the endpoint.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Creates a success response envelope.
        /// </summary>
        public static ApiResponse<T> Ok(T data, string? message = null) => new()
        {
            Success = true,
            Message = message,
            Data = data
        };

        /// <summary>
        /// Creates a failure response envelope.
        /// </summary>
        public static ApiResponse<T> Fail(string message) => new()
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}
