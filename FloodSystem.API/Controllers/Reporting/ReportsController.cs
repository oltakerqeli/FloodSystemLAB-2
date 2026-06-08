using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FloodSystem.API.Services.Reporting;
using FloodSystem.API.DTOs.Reporting;

namespace FloodSystem.API.Controllers.Reporting;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service)
    {
        _service = service;
    }

    [HttpPost("flood")]
    public async Task<IActionResult> CreateFloodReport([FromBody] CreateFloodReportDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.CreateFloodReportAsync(dto, userId);
        return Ok(result);
    }

    [HttpPost("drain")]
    public async Task<IActionResult> CreateDrainReport([FromForm] CreateDrainReportDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.CreateDrainReportAsync(dto, userId);
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
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Authority")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] int statusId, [FromQuery] string type)
    {
        await _service.UpdateReportStatusAsync(id, statusId, type);
        return NoContent();
    }
}