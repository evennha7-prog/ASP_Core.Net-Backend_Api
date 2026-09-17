using backend_api.Data;
using backend_api.Models;
using backend_api.Repositories.Interfaces;
using Dapper;

namespace backend_api.Repositories
{
    /// <summary>
    /// Implements data access operations for the "Notes" table using Dapper and parameterized SQL.
    /// Strictly filters queries by UserId to enforce multi-tenant isolation per user.
    /// </summary>
    public class NoteRepository : INoteRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        /// <summary>
        /// Injects the SQL connection factory.
        /// </summary>
        /// <param name="connectionFactory">Factory for creating IDbConnection instances.</param>
        public NoteRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Retrieves all notes belonging to the specified user, optionally filtered by keyword and sorted.
        /// </summary>
        /// <param name="userId">Owner user ID.</param>
        /// <param name="search">Search string matched against Title and Content.</param>
        /// <param name="sort">Sorting mode: 'oldest', 'title_asc', 'title_desc', 'updated', or default newest.</param>
        public async Task<IEnumerable<Note>> GetAllByUserIdAsync(int userId, string? search = null, string? sort = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT * FROM Notes WHERE UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);

            // Dynamic filter for search keywords
            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += " AND (Title LIKE @Search OR Content LIKE @Search)";
                parameters.Add("Search", $"%{search.Trim()}%");
            }

            // Append appropriate ORDER BY clause
            sql += sort?.ToLowerInvariant() switch
            {
                "oldest" => " ORDER BY CreatedAt ASC",
                "title_asc" => " ORDER BY Title ASC",
                "title_desc" => " ORDER BY Title DESC",
                "updated" => " ORDER BY UpdatedAt DESC",
                _ => " ORDER BY CreatedAt DESC" // default: newest first
            };

            return await connection.QueryAsync<Note>(sql, parameters);
        }

        /// <summary>
        /// Retrieves a specific note by ID, ensuring it belongs to the given user ID.
        /// </summary>
        public async Task<Note?> GetByIdAsync(int id, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Notes WHERE Id = @Id AND UserId = @UserId";
            return await connection.QuerySingleOrDefaultAsync<Note>(sql, new { Id = id, UserId = userId });
        }

        /// <summary>
        /// Inserts a new note record and returns the newly generated identity ID.
        /// </summary>
        public async Task<int> CreateAsync(Note note)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
                VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var now = DateTime.UtcNow;
            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                note.UserId,
                Title = note.Title.Trim(),
                note.Content,
                CreatedAt = note.CreatedAt == default ? now : note.CreatedAt,
                UpdatedAt = note.UpdatedAt == default ? now : note.UpdatedAt
            });
        }

        /// <summary>
        /// Updates an existing note belonging to the user and updates its UpdatedAt timestamp.
        /// </summary>
        public async Task<bool> UpdateAsync(Note note)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Notes 
                SET Title = @Title, 
                    Content = @Content, 
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND UserId = @UserId";

            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                note.Id,
                note.UserId,
                Title = note.Title.Trim(),
                note.Content,
                UpdatedAt = DateTime.UtcNow
            });

            return affectedRows > 0;
        }

        /// <summary>
        /// Permanently deletes a note by ID if owned by the specified user.
        /// </summary>
        public async Task<bool> DeleteAsync(int id, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
            return affectedRows > 0;
        }
    }
}
