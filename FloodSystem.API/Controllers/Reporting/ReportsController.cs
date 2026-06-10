using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FloodSystem.API.Services.Reporting;
using FloodSystem.API.DTOs.Reporting;
using FloodSystem.API.Hubs;
using FloodSystem.API.Models.Dashboard;
using FloodSystem.API.Repositories.Dashboard;
using Microsoft.AspNetCore.SignalR;

namespace FloodSystem.API.Controllers.Reporting;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IDashboardRepository _dashboardRepo;

    public ReportsController(
        IReportService service,
        IHubContext<NotificationHub> hubContext,
        IDashboardRepository dashboardRepo)
    {
        _service = service;
        _hubContext = hubContext;
        _dashboardRepo = dashboardRepo;
    }

    [HttpPost("flood")]
    public async Task<IActionResult> CreateFloodReport([FromBody] CreateFloodReportDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.CreateFloodReportAsync(dto, userId);
        await _dashboardRepo.CreateNotificationAsync(new Notification
        {
            UserId = null,
            Type = "report",
            Title = "New Flood Report",
            Message = $"{dto.LocationName} - {dto.Severity}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        try
        {
            await _hubContext.Clients.Group("Admins").SendAsync("NewReport", new
            {
                id = result.Id,
                type = "Flood",
                location = dto.LocationName,
                severity = dto.Severity,
                description = dto.Description,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex) { }

        return Ok(result);
    }

    [HttpPost("drain")]
    public async Task<IActionResult> CreateDrainReport([FromForm] CreateDrainReportDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.CreateDrainReportAsync(dto, userId);
        await _dashboardRepo.CreateNotificationAsync(new Notification
        {
            UserId = null,
            Type = "report",
            Title = "New Drain Report",
            Message = $"{dto.Street ?? "Unknown"} - {dto.Severity}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        try
        {
            await _hubContext.Clients.Group("Admins").SendAsync("NewReport", new
            {
                id = result.Id,
                type = "Drain",
                location = dto.Street ?? "Unknown",
                severity = dto.Severity,
                description = dto.Description,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex) { }

        return Ok(result);
    }

    [HttpGet("flood")]
    public async Task<IActionResult> GetFloodReports()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _service.GetAllFloodReportsAsync(userId));
    }

    [HttpGet("drain")]
    public async Task<IActionResult> GetDrainReports()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _service.GetAllDrainReportsAsync(userId));
    }

    [HttpGet("flood/all")]
    [Authorize(Roles = "Admin,Authority")]
    public async Task<IActionResult> GetAllFloodReports()
        => Ok(await _service.GetAllFloodReportsAsync());

    [HttpGet("drain/all")]
    [Authorize(Roles = "Admin,Authority")]
    public async Task<IActionResult> GetAllDrainReports()
        => Ok(await _service.GetAllDrainReportsAsync());

  [HttpPatch("{id}/status")]
    [Authorize(Roles = "Authority")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] int statusId, [FromQuery] string type)
    {
        var ownerUserId = await _service.GetReportOwnerIdAsync(id, type);

        await _service.UpdateReportStatusAsync(id, statusId, type);

        var statusName = statusId switch
        {
            1 => "Pending",
            2 => "In Progress",
            3 => "Resolved",
            _ => "Updated"
        };

        await _dashboardRepo.CreateNotificationAsync(new Notification
        {
            UserId = ownerUserId,
            Type = "status",
            Title = $"Report #{id} Status Changed",
            Message = $"{type} Report #{id} is now {statusName}",
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        if (ownerUserId.HasValue)
        {
            try
            {
                await _hubContext.Clients.Group($"user_{ownerUserId.Value}").SendAsync("StatusChanged", new
                {
                    reportId = id,
                    reportType = type,
                    newStatus = statusName,
                    message = $"{type} Report #{id} is now {statusName}",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR send failed: {ex.Message}");
            }
        }

        return NoContent();
    }
}