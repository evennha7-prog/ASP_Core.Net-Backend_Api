using backend_api.DTOs.Auth;
using backend_api.Exceptions;
using backend_api.Helpers;
using backend_api.Models;
using backend_api.Repositories.Interfaces;
using backend_api.Services.Interfaces;

namespace backend_api.Services
{
    /// <summary>
    /// Implements authentication business logic including input validation,
    /// duplicate checks, BCrypt password hashing, credential verification, and JWT generation.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        /// <summary>
        /// Constructor injecting the user repository and JWT helper service.
        /// </summary>
        /// <param name="userRepository">Data access repository for users.</param>
        /// <param name="jwtHelper">Utility for generating and signing JWT tokens.</param>
        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Registers a new user:
        /// 1. Verifies username uniqueness.
        /// 2. Verifies email uniqueness.
        /// 3. Hashes the password with BCrypt.
        /// 4. Stores the user in the database.
        /// 5. Issues a JWT token for immediate login.
        /// </summary>
        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            // Enforce username uniqueness
            if (await _userRepository.ExistsByUsernameAsync(request.Username))
            {
                throw new ConflictException("Username is already taken.");
            }

            // Enforce email uniqueness
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new ConflictException("Email is already registered.");
            }

            // Hash password with BCrypt (salted, work factor 11)
            var passwordHash = PasswordHasher.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            // Persist user to database
            var userId = await _userRepository.CreateAsync(user);
            user.Id = userId;

            // Generate JWT authentication token
            var (token, expiration) = _jwtHelper.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                Expiration = expiration,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                }
            };
        }

        /// <summary>
        /// Authenticates user credentials:
        /// 1. Looks up the user by email or username.
        /// 2. Compares the supplied password against the stored BCrypt hash.
        /// 3. Generates a signed JWT upon success.
        /// </summary>
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Find user by either email or username
            var user = await _userRepository.GetByEmailOrUsernameAsync(request.EmailOrUsername);
            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid credentials. Please check your username/email and password.");
            }

            // Generate JWT authentication token
            var (token, expiration) = _jwtHelper.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                Expiration = expiration,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                }
            };
        }

        /// <summary>
        /// Retrieves the public profile of a user by ID.
        /// </summary>
        public async Task<UserDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }
    }
}
