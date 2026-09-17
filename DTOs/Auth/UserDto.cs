namespace backend_api.DTOs.Auth
{
    /// <summary>
    /// Safe public user representation returned in API responses (omits sensitive data like password hash).
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Unique identifier of the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username of the user.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
