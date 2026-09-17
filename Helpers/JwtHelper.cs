using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_api.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend_api.Helpers
{
    /// <summary>
    /// Helper service responsible for issuing and validating JSON Web Tokens (JWT)
    /// for stateless user authentication across API requests.
    /// </summary>
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes the JWT helper with the application configuration settings.
        /// </summary>
        /// <param name="configuration">Configuration providing JWT Secret Key, Issuer, Audience, and Expiration duration.</param>
        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a signed JWT token containing standard user claims (NameIdentifier, Name, Email, Jti).
        /// </summary>
        /// <param name="user">The authenticated user entity whose claims should be embedded.</param>
        /// <returns>A tuple containing the serialized token string and its UTC expiration DateTime.</returns>
        public (string Token, DateTime Expiration) GenerateToken(User user)
        {
            // Read JWT settings from appsettings.json, with robust fallbacks
            var jwtKey = _configuration["Jwt:Key"]
                ?? "A_Very_Strong_Secret_Key_For_Notes_Api_Fullstack_Test_2026_JWT!";
            var issuer = _configuration["Jwt:Issuer"] ?? "NotesApi";
            var audience = _configuration["Jwt:Audience"] ?? "NotesApiAudience";
            var durationMinutes = double.TryParse(_configuration["Jwt:DurationInMinutes"], out var minutes)
                ? minutes
                : 1440; // Default 24 hours (1440 minutes)

            // Create symmetric security key and HMAC-SHA256 signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(durationMinutes);

            // User claims packaged into the token payload
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token identifier
            };

            // Build the JWT descriptor
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }

        /// <summary>
        /// Validates an incoming JWT string and returns its ClaimsPrincipal if valid and unexpired.
        /// </summary>
        /// <param name="token">Raw JWT string extracted from the HTTP Authorization header.</param>
        /// <returns>The validated ClaimsPrincipal or null if validation fails.</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var jwtKey = _configuration["Jwt:Key"]
                ?? "A_Very_Strong_Secret_Key_For_Notes_Api_Fullstack_Test_2026_JWT!";
            var issuer = _configuration["Jwt:Issuer"] ?? "NotesApi";
            var audience = _configuration["Jwt:Audience"] ?? "NotesApiAudience";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1) // 1 minute leeway for minor clock differences
                }, out _);

                return principal;
            }
            catch
            {
                // Token expired, signature invalid, or malformed
                return null;
            }
        }
    }
}
