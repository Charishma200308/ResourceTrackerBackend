using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceTrackerBackend.DAO;
using ResourceTrackerBackend.Models;

namespace ResourceTrackerBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] Login loginDto)
        {
            var user = _authService.GetUserByEmail(loginDto.Email);
            if (user == null || !_authService.ValidatePassword(user, loginDto.Password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] Register dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email
            };

            var result = _authService.RegisterUser(user, dto.Password);
            if (!result)
            {
                return BadRequest(new { message = "User registration failed. Possibly duplicate email." });
            }

            return Ok(new { message = "User registered successfully" });
        }


    }
}
