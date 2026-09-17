namespace backend_api.Models
{
    /// <summary>
    /// Represents a registered user domain model mapped to the "Users" table in SQL Server.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique primary key identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique username chosen by the user during registration.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Unique email address for the user account.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// One-way BCrypt salted hash of the user's password. Never store plaintext passwords!
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// UTC timestamp when the user account was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
