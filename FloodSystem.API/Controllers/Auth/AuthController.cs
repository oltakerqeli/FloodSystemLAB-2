using FloodSystem.API.DTOs.Auth;
using FloodSystem.API.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FloodSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (result == "Email already exists.")
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(new
            {
                message = "Login successful.",
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshAsync(dto.RefreshToken);

            if (result == null)
                return Unauthorized(new { message = "Invalid or expired refresh token." });

            return Ok(new
            {
                message = "Token refreshed successfully.",
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRequestDto dto)
        {
            var result = await _authService.LogoutAsync(dto.RefreshToken);

            if (!result)
                return BadRequest(new { message = "Invalid or already revoked refresh token." });

            return Ok(new { message = "Logout successful." });
        }
    }
}