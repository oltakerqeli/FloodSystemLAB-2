using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FloodSystem.API.Repositories.Dashboard;

namespace FloodSystem.API.Controllers.Dashboard;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IDashboardRepository _repo;

    public NotificationsController(IDashboardRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdminOrAuthority = User.IsInRole("Admin") || User.IsInRole("Authority");

        var notifications = await _repo.GetNotificationsForUserAsync(userId, isAdminOrAuthority);

        return Ok(notifications.Select(n => new
        {
            id = n.Id,
            type = n.Type,
            title = n.Title,
            message = n.Message,
            isRead = n.IsRead,
            createdAt = n.CreatedAt
        }));
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _repo.MarkNotificationAsReadAsync(id);
        return NoContent();
    }
}