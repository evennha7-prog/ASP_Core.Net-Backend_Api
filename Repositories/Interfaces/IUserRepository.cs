using backend_api.Models;

namespace backend_api.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface abstracting direct database operations on the "Users" table.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their unique primary key.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>The User model if found; otherwise, null.</returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a user by email address (case-insensitive).
        /// </summary>
        /// <param name="email">Email address to find.</param>
        /// <returns>The User model if found; otherwise, null.</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Retrieves a user by username (case-insensitive).
        /// </summary>
        /// <param name="username">Username to find.</param>
        /// <returns>The User model if found; otherwise, null.</returns>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Retrieves a user by either their email address or username. Useful for flexible login.
        /// </summary>
        /// <param name="identifier">Email or username entered by the user.</param>
        /// <returns>The User model if found; otherwise, null.</returns>
        Task<User?> GetByEmailOrUsernameAsync(string identifier);

        /// <summary>
        /// Inserts a new user record into the database and returns the generated primary key.
        /// </summary>
        /// <param name="user">User entity with username, email, and password hash.</param>
        /// <returns>The newly created user's primary key ID.</returns>
        Task<int> CreateAsync(User user);

        /// <summary>
        /// Checks whether an email address already exists in the database.
        /// </summary>
        /// <param name="email">Email to verify.</param>
        /// <returns>True if already registered; otherwise, false.</returns>
        Task<bool> ExistsByEmailAsync(string email);

        /// <summary>
        /// Checks whether a username is already taken.
        /// </summary>
        /// <param name="username">Username to verify.</param>
        /// <returns>True if already in use; otherwise, false.</returns>
        Task<bool> ExistsByUsernameAsync(string username);
    }
}
