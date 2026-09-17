using Dapper;
using Microsoft.Data.SqlClient;

namespace backend_api.Data
{
    /// <summary>
    /// Handles database startup automation:
    /// 1. Ensures the target database (Backend_DB) exists on the SQL Server instance.
    /// 2. Creates the necessary tables (Users, Notes) and foreign key constraints / indexes if they do not exist.
    /// 3. Seeds an initial demo user and sample notes if the database is newly initialized.
    /// </summary>
    public class DatabaseConnection
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseConnection> _logger;

        /// <summary>
        /// Constructor for injecting configuration and logger dependencies.
        /// </summary>
        /// <param name="configuration">Access to application settings and connection strings.</param>
        /// <param name="logger">Logger for recording initialization events and potential errors.</param>
        public DatabaseConnection(IConfiguration configuration, ILogger<DatabaseConnection> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously verifies and initializes the database schema, creating tables and sample data as needed.
        /// Called automatically during application startup in Program.cs.
        /// </summary>
        public async Task InitializeDatabaseAsync()
        {
            try
            {
                // Retrieve connection strings from configuration
                var masterConnStr = _configuration.GetConnectionString("MasterConnection");
                var defaultConnStr = _configuration.GetConnectionString("DefaultConnection")
                    ?? "Server=.\\SQLEXPRESS;Database=Backend_DB;Integrated Security=True;TrustServerCertificate=True;";

                // Parse the target database catalog name (e.g., "Backend_DB")
                var connBuilder = new SqlConnectionStringBuilder(defaultConnStr);
                var targetDb = string.IsNullOrWhiteSpace(connBuilder.InitialCatalog) ? "Backend_DB" : connBuilder.InitialCatalog;

                // -------------------------------------------------------------
                // STEP 1: Ensure target database exists (for local SQL Server)
                // On shared cloud databases (like MonsterASP, Azure), the database is pre-created and master access is restricted.
                // -------------------------------------------------------------
                if (!string.IsNullOrWhiteSpace(masterConnStr))
                {
                    try
                    {
                        using (var masterConn = new SqlConnection(masterConnStr))
                        {
                            await masterConn.OpenAsync();
                            var checkDbSql = $@"
                                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{targetDb}')
                                BEGIN
                                    CREATE DATABASE [{targetDb}];
                                END";
                            await masterConn.ExecuteAsync(checkDbSql);
                            _logger.LogInformation("Database '{TargetDb}' verified/created successfully.", targetDb);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Bypassing master database check (running on hosted/managed cloud database): {Message}", ex.Message);
                    }
                }

                // -------------------------------------------------------------
                // STEP 2: Ensure tables and indexes exist in target database
                // -------------------------------------------------------------
                using (var appConn = new SqlConnection(defaultConnStr))
                {
                    await appConn.OpenAsync();

                    var createTablesSql = @"
                        -- Create Users table if not already present
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                        BEGIN
                            CREATE TABLE Users (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                Username NVARCHAR(100) NOT NULL UNIQUE,
                                Email NVARCHAR(256) NOT NULL UNIQUE,
                                PasswordHash NVARCHAR(MAX) NOT NULL,
                                CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                            );
                        END;

                        -- Create Notes table with foreign key reference to Users
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notes')
                        BEGIN
                            CREATE TABLE Notes (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                UserId INT NOT NULL,
                                Title NVARCHAR(255) NOT NULL,
                                Content NVARCHAR(MAX) NULL,
                                CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                                UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                                CONSTRAINT FK_Notes_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                            );

                            -- Indexes to speed up queries by user and chronological sorting
                            CREATE INDEX IX_Notes_UserId ON Notes(UserId);
                            CREATE INDEX IX_Notes_CreatedAt ON Notes(CreatedAt DESC);
                        END;
                    ";

                    await appConn.ExecuteAsync(createTablesSql);
                    _logger.LogInformation("Database tables 'Users' and 'Notes' verified/created successfully in '{TargetDb}'.", targetDb);

                    // -------------------------------------------------------------
                    // STEP 3: Seed initial demo user and sample notes if DB is empty
                    // -------------------------------------------------------------
                    var userCountSql = "SELECT COUNT(1) FROM Users";
                    var userCount = await appConn.ExecuteScalarAsync<int>(userCountSql);
                    if (userCount == 0)
                    {
                        // Default credentials: Username = testadmin, Password = AdminPass123!
                        var passwordHash = BCrypt.Net.BCrypt.HashPassword("AdminPass123!", workFactor: 11);
                        var insertUserSql = @"
                            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
                            VALUES (@Username, @Email, @PasswordHash, GETUTCDATE());
                            SELECT CAST(SCOPE_IDENTITY() as int);";

                        var userId = await appConn.ExecuteScalarAsync<int>(insertUserSql, new
                        {
                            Username = "testadmin",
                            Email = "admin@test.com",
                            PasswordHash = passwordHash
                        });

                        // Insert sample introductory notes for the seeded user
                        var insertNotesSql = @"
                            INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
                            VALUES 
                            (@UserId, 'Welcome to NotesHub!', 'Your Fullstack Notes application is connected to SQL Server (Backend_DB) and Vue 3 frontend!' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) + 'Features available:' + CHAR(13) + CHAR(10) + '- Create notes with Title & Content' + CHAR(13) + CHAR(10) + '- Auto timestamps for CreatedAt and UpdatedAt' + CHAR(13) + CHAR(10) + '- Search and sort notes' + CHAR(13) + CHAR(10) + '- Edit and delete with confirmation', GETUTCDATE(), GETUTCDATE()),
                            (@UserId, 'Architecture & Tech Stack', 'Backend: ASP.NET Core (.NET 10), Dapper, SQL Server' + CHAR(13) + CHAR(10) + 'Frontend: Vue 3, TypeScript, Pinia, Tailwind CSS' + CHAR(13) + CHAR(10) + 'Security: JWT Bearer Tokens & BCrypt password hashing', DATEADD(minute, -10, GETUTCDATE()), DATEADD(minute, -10, GETUTCDATE())),
                            (@UserId, 'Weekly Priorities Checklist', '1. Complete API integration test' + CHAR(13) + CHAR(10) + '2. Verify database connection in Navicat' + CHAR(13) + CHAR(10) + '3. Enjoy taking notes!', DATEADD(minute, -30, GETUTCDATE()), DATEADD(minute, -30, GETUTCDATE()));
                        ";

                        await appConn.ExecuteAsync(insertNotesSql, new { UserId = userId });
                        _logger.LogInformation("Seeded default demo user (testadmin / AdminPass123!) and sample notes into '{TargetDb}'.", targetDb);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database schema.");
                throw;
            }
        }
    }
}
