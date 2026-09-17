using System.Data;
using Microsoft.Data.SqlClient;

namespace backend_api.Data
{
    /// <summary>
    /// Contract for creating database connections and retrieving the configured connection string.
    /// Used across repositories for dependency injection and testability.
    /// </summary>
    public interface ISqlConnectionFactory
    {
        /// <summary>
        /// Creates and returns a new closed SQL Server database connection.
        /// </summary>
        /// <returns>An instance of <see cref="IDbConnection"/> connected to SQL Server.</returns>
        IDbConnection CreateConnection();

        /// <summary>
        /// Gets the raw connection string currently in use.
        /// </summary>
        string ConnectionString { get; }
    }

    /// <summary>
    /// Factory responsible for instantiating SQL Server connections using the connection string
    /// defined in the application configuration (appsettings.json).
    /// </summary>
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes the connection factory by reading the "DefaultConnection" string from configuration.
        /// </summary>
        /// <param name="configuration">The application configuration provider.</param>
        /// <exception cref="InvalidOperationException">Thrown if "DefaultConnection" is missing.</exception>
        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        /// <summary>
        /// Gets the connection string configured for the application database.
        /// </summary>
        public string ConnectionString => _connectionString;

        /// <summary>
        /// Creates a new instance of <see cref="SqlConnection"/> using the configured connection string.
        /// The caller is responsible for disposing the connection (typically via a `using` statement).
        /// </summary>
        /// <returns>A new <see cref="IDbConnection"/> instance.</returns>
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
