using backend_api.Data;
using backend_api.Extensions;
using backend_api.Middleware;
using Scalar.AspNetCore;

namespace backend_api
{
    /// <summary>
    /// Application entry point:
    /// Bootstraps ASP.NET Core Web API, configures Dependency Injection,
    /// performs automated database schema migration, and registers the middleware pipeline.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main application method invoked on process start.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Name of the CORS policy used by the Vue frontend
            const string corsPolicyName = "AllowFrontend";

            // -------------------------------------------------------------
            // 1. Service Registration (Dependency Injection)
            // -------------------------------------------------------------
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // Custom extension methods for DI (defined in Extensions/ServiceExtensions.cs)
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddCorsPolicy(corsPolicyName);

            var app = builder.Build();

            // -------------------------------------------------------------
            // 2. Database Auto-Migration & Seed Data
            // -------------------------------------------------------------
            // Automatically creates the Backend_DB database, Users table, Notes table,
            // and demo seed data if they do not already exist on SQL Server.
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseConnection>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    logger.LogInformation("Initializing database schema...");
                    await dbInitializer.InitializeDatabaseAsync();
                    logger.LogInformation("Database schema initialized successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to initialize database schema on startup.");
                }
            }

            // -------------------------------------------------------------
            // 3. HTTP Request Processing Pipeline
            // -------------------------------------------------------------
            // Global exception handling: catches errors and produces standard JSON error responses
            app.UseMiddleware<ExceptionMiddleware>();

            // Development API Documentation (OpenAPI and Scalar)
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            // Redirect HTTP requests to HTTPS
            app.UseHttpsRedirection();

            // Enable Cross-Origin Resource Sharing for the Vue 3 frontend
            app.UseCors(corsPolicyName);

            // Authentication & Custom JWT Middleware
            app.UseAuthentication();
            app.UseMiddleware<JwtMiddleware>();
            app.UseAuthorization();

            // Map controller routes (e.g., api/auth, api/notes)
            app.MapControllers();

            // Run the web server
            await app.RunAsync();
        }
    }
}
