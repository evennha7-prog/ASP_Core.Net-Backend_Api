using System.Text;
using backend_api.Data;
using backend_api.Helpers;
using backend_api.Repositories;
using backend_api.Repositories.Interfaces;
using backend_api.Services;
using backend_api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace backend_api.Extensions
{
    /// <summary>
    /// Extension methods on <see cref="IServiceCollection"/> to keep <c>Program.cs</c> clean,
    /// organized, and modular.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registers data access layer, helpers, repositories, and business services into the DI container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The modified service collection for method chaining.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // --- Data Layer ---
            // Singleton: Single factory and DB connection manager instance shared across the entire app lifetime
            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddSingleton<DatabaseConnection>();

            // --- Helpers ---
            // Singleton: JWT token creation helper does not hold per-request state
            services.AddSingleton<JwtHelper>();

            // --- Repositories ---
            // Scoped: Created once per HTTP request
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();

            // --- Services ---
            // Scoped: Encapsulates business logic per HTTP request
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INoteService, NoteService>();

            return services;
        }

        /// <summary>
        /// Configures JWT Bearer authentication scheme for ASP.NET Core security pipeline.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration to read JWT keys.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"]
                ?? "A_Very_Strong_Secret_Key_For_Notes_Api_Fullstack_Test_2026_JWT!";
            var issuer = configuration["Jwt:Issuer"] ?? "NotesApi";
            var audience = configuration["Jwt:Audience"] ?? "NotesApiAudience";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in strict production HTTPS environments
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Strict expiration check without extra grace period
                };
            });

            return services;
        }

        /// <summary>
        /// Configures Cross-Origin Resource Sharing (CORS) policy to allow frontend access.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="policyName">The identifier for the CORS policy.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, string policyName)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}
