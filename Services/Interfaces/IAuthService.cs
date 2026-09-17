using backend_api.DTOs.Auth;

namespace backend_api.Services.Interfaces
{
    /// <summary>
    /// Service contract handling user registration, authentication, and profile retrieval.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user account, verifies uniqueness of username and email, hashes the password, and returns a JWT.
        /// </summary>
        /// <param name="request">Registration payload containing username, email, and password.</param>
        /// <returns>LoginResponse containing the generated JWT token and user info.</returns>
        Task<LoginResponse> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Authenticates an existing user by email/username and password, returning a JWT token upon success.
        /// </summary>
        /// <param name="request">Login payload with credentials.</param>
        /// <returns>LoginResponse containing the JWT token and user details.</returns>
        Task<LoginResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Retrieves the current authenticated user's public profile by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user to look up.</param>
        /// <returns>UserDto if found; otherwise, null.</returns>
        Task<UserDto?> GetCurrentUserAsync(int userId);
    }
}
