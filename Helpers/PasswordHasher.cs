namespace backend_api.Helpers
{
    /// <summary>
    /// Utility class providing secure password hashing and verification using the BCrypt algorithm.
    /// BCrypt incorporates a random salt and adaptive work factor to protect against rainbow table and brute-force attacks.
    /// </summary>
    public static class PasswordHasher
    {
        /// <summary>
        /// Hashes a plain text password using BCrypt with a standard work factor of 11.
        /// </summary>
        /// <param name="password">The plain text password to hash.</param>
        /// <returns>A secure BCrypt salted hash string.</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }

        /// <summary>
        /// Verifies whether an entered plain text password matches the stored BCrypt hash.
        /// </summary>
        /// <param name="password">The entered plain text password.</param>
        /// <param name="passwordHash">The previously hashed password stored in the database.</param>
        /// <returns>True if the password matches the hash; otherwise, false.</returns>
        public static bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch
            {
                // In case of invalid salt/hash format or corrupt data, safely fail authentication
                return false;
            }
        }
    }
}
