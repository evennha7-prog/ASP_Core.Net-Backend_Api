using System.ComponentModel.DataAnnotations;

namespace backend_api.DTOs.Auth
{
    /// <summary>
    /// Request payload for registering a new user account.
    /// Includes validation annotations for username length, email formatting, and minimum password complexity.
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Desired username (must be 3-50 characters long).
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Valid email address for the new account (must be unique).
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password chosen by the user (must be at least 6 characters).
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;
    }
}
