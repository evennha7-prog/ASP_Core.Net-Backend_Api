using System.Security.Claims;
using backend_api.DTOs.Auth;
using backend_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_api.Controllers
{
    /// <summary>
    /// API Controller providing authentication endpoints:
    /// - POST /api/auth/register : Register a new user account
    /// - POST /api/auth/login    : Authenticate and obtain a JWT bearer token
    /// - GET  /api/auth/me       : Get current authenticated user profile
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Injects the authentication service.
        /// </summary>
        /// <param name="authService">Service handling authentication business logic.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="request">Payload containing username, email, and password.</param>
        /// <returns>HTTP 201 with JWT token and user details, or HTTP 400 on validation error.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.RegisterAsync(request);
            return StatusCode(StatusCodes.Status201Created, response);
        }

        /// <summary>
        /// Logs in an existing user with email/username and password.
        /// </summary>
        /// <param name="request">Payload containing credentials.</param>
        /// <returns>HTTP 200 with JWT token and user info, or HTTP 401 on bad credentials.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves profile details of the currently logged-in user.
        /// Requires a valid JWT Bearer token in the Authorization header.
        /// </summary>
        /// <returns>HTTP 200 with UserDto, or HTTP 401 if unauthorized.</returns>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Extract the user ID claim embedded in the validated JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _authService.GetCurrentUserAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
