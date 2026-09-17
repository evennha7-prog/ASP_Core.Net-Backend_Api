using backend_api.Data;
using backend_api.Models;
using backend_api.Repositories.Interfaces;
using Dapper;

namespace backend_api.Repositories
{
    /// <summary>
    /// Implements data access operations for the "Users" table using Dapper and raw SQL.
    /// Provides parameterized queries to guard against SQL injection vulnerabilities.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        /// <summary>
        /// Injects the SQL connection factory to create short-lived database connections per query.
        /// </summary>
        /// <param name="connectionFactory">Factory for creating IDbConnection instances.</param>
        public UserRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Finds a user by primary key ID.
        /// </summary>
        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Users WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
        }

        /// <summary>
        /// Finds a user by email address using a case-insensitive check.
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Users WHERE LOWER(Email) = LOWER(@Email)";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email.Trim() });
        }

        /// <summary>
        /// Finds a user by username using a case-insensitive check.
        /// </summary>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Users WHERE LOWER(Username) = LOWER(@Username)";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username.Trim() });
        }

        /// <summary>
        /// Finds a user matching either username or email address.
        /// Enables users to log in with either identifier.
        /// </summary>
        public async Task<User?> GetByEmailOrUsernameAsync(string identifier)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT * FROM Users 
                WHERE LOWER(Email) = LOWER(@Identifier) OR LOWER(Username) = LOWER(@Identifier)";
            return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Identifier = identifier.Trim() });
        }

        /// <summary>
        /// Inserts a new user record and retrieves the newly assigned IDENTITY id.
        /// </summary>
        public async Task<int> CreateAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
                VALUES (@Username, @Email, @PasswordHash, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                Username = user.Username.Trim(),
                Email = user.Email.Trim().ToLowerInvariant(),
                user.PasswordHash,
                CreatedAt = user.CreatedAt == default ? DateTime.UtcNow : user.CreatedAt
            });
        }

        /// <summary>
        /// Checks if any user record exists with the specified email address.
        /// </summary>
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Users WHERE LOWER(Email) = LOWER(@Email)";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email.Trim() });
            return count > 0;
        }

        /// <summary>
        /// Checks if any user record exists with the specified username.
        /// </summary>
        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Users WHERE LOWER(Username) = LOWER(@Username)";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Username = username.Trim() });
            return count > 0;
        }
    }
}
