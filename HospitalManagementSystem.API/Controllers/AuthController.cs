using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Login and receive a JWT token</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _authService.LoginAsync(dto);
            return result == null
                ? Unauthorized("Invalid username or password.")
                : Ok(result);
        }

        /// <summary>Register a new user (Admin only)</summary>
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (result, error) = await _authService.RegisterAsync(dto);
            if (error != null) return BadRequest(error);
            return Ok(result);
        }
    }
}
