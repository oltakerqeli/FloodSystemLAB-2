using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FloodSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthTestController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok(new { message = "This endpoint is public." });
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);

            return Ok(new
            {
                message = "You are authenticated.",
                name,
                email
            });
        }

        [Authorize(Roles = "User")]
        [HttpGet("user-only")]
        public IActionResult UserOnly()
        {
            return Ok(new { message = "Only users with User role can access this endpoint." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "Only Admins can access this endpoint." });
        }

        [Authorize(Roles = "Authority")]
        [HttpGet("authority-only")]
        public IActionResult AuthorityOnly()
        {
            return Ok(new { message = "Only Authority users can access this endpoint." });
        }
    }
}