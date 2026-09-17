using System.ComponentModel.DataAnnotations;

namespace backend_api.DTOs.Auth
{
    /// <summary>
    /// Request payload for authenticating an existing user.
    /// Supports logging in using either their registered email address or username.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// The user's registered email address or username.
        /// </summary>
        [Required(ErrorMessage = "Email or username is required")]
        public string EmailOrUsername { get; set; } = string.Empty;

        /// <summary>
        /// The plain text password entered by the user.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
