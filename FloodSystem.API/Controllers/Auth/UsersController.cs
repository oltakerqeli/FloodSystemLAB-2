using FloodSystem.API.DTOs.Auth;
using FloodSystem.API.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FloodSystem.API.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ManageUsers")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var result = await _userService.DeactivateUserAsync(id);

            if (result == "User not found.")
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var result = await _userService.ActivateUserAsync(id);

            if (result == "User not found.")
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }

        [HttpPut("{id}/assign-role")]
        public async Task<IActionResult> AssignRole(int id, AssignRoleDto dto)
        {
            var result = await _userService.AssignRoleAsync(id, dto);

            if (result == "User not found." || result == "Role not found.")
                return NotFound(new { message = result });

            if (result == "User already has this role.")
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }
    }
}