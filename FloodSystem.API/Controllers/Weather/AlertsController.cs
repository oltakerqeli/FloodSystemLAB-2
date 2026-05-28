using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FloodSystem.API.Services.Weather;

namespace FloodSystem.API.Controllers.Weather
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertsController : ControllerBase
    {
        private readonly AlertService _alertService;

        public AlertsController(AlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var alerts = await _alertService.GetAllAlertsAsync();
            return Ok(alerts);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveAlerts()
        {
            var alerts = await _alertService.GetActiveAlertsAsync();
            return Ok(alerts);
        }

        [HttpGet("location/{locationId}")]
        public async Task<IActionResult> GetByLocation(int locationId)
        {
            var alerts = await _alertService.GetAlertsByLocationAsync(locationId);
            return Ok(alerts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var alert = await _alertService.GetAlertByIdAsync(id);
            if (alert == null)
                return NotFound(new { message = "Alert not found" });

            return Ok(alert);
        }
    }
}