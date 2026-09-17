namespace backend_api.DTOs.Auth
{
    /// <summary>
    /// Response payload returned after successful login or registration containing the JWT token.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Signed JSON Web Token (JWT) string to be used as Bearer token in subsequent HTTP requests.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// UTC expiration date and time of the JWT token.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Authenticated user's profile details.
        /// </summary>
        public UserDto User { get; set; } = new();
    }
}
