using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FloodSystem.API.Services.Dashboard;
using FloodSystem.API.DTOs.Dashboard;

namespace FloodSystem.API.Controllers.Dashboard;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet("audit-logs")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAuditLogs()
        => Ok(await _service.GetAllAuditLogsAsync());

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _service.GetNotificationsByUserAsync(userId));
    }

    [HttpPost("notifications")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto dto)
    {
        await _service.CreateNotificationAsync(dto);
        return Ok();
    }

    [HttpPatch("notifications/{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _service.MarkNotificationAsReadAsync(id);
        return NoContent();
    }

    [HttpGet("settings")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSettings()
        => Ok(await _service.GetAllSettingsAsync());

    [HttpPatch("settings/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSetting(int id, [FromBody] string value)
    {
        await _service.UpdateSettingAsync(id, value);
        return NoContent();
    }

    [HttpPost("export")]
    public async Task<IActionResult> CreateExport([FromBody] CreateExportDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.CreateExportAsync(dto, userId);
        return Ok(result);
    }

    [HttpPost("import")]
    public async Task<IActionResult> CreateImport([FromBody] CreateImportDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.CreateImportAsync(dto, userId);
        return Ok(result);
    }
}