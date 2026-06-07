using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FloodSystem.API.DTOs.Weather;
using FloodSystem.API.Services.Weather;
using FloodSystem.API.Data;
using Microsoft.EntityFrameworkCore;
using FloodSystem.API.Models.Weather;  // ← Shto këtë rresht

namespace FloodSystem.API.Controllers.Weather
{
    [ApiController]
    [Route("api/[controller]")]
   [Authorize]
    public class ZonesController : ControllerBase
    {
        private readonly ZoneService _zoneService;
        private readonly ApplicationDbContext _context;

        public ZonesController(ZoneService zoneService, ApplicationDbContext context)
        {
            _zoneService = zoneService;
            _context = context;
        }

        // ==================== METODAT EKZISTUESE ====================

        [HttpGet]
         [Authorize] 
        public async Task<IActionResult> GetAll()
        {
            var zones = await _zoneService.GetAllZonesAsync();
            return Ok(zones);
        }

        [HttpGet("{id}")]
         [Authorize] 
        public async Task<IActionResult> GetById(int id)
        {
            var zone = await _zoneService.GetZoneByIdAsync(id);
            if (zone == null)
                return NotFound(new { message = "Zone not found" });

            return Ok(zone);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Authority")]
        public async Task<IActionResult> Create([FromBody] CreateZoneDto dto)
        {
            try
            {
                var zone = await _zoneService.CreateZoneAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Authority")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateZoneDto dto)
        {
            var zone = await _zoneService.UpdateZoneAsync(id, dto);
            if (zone == null)
                return NotFound(new { message = "Zone not found" });

            return Ok(zone);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Authority")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _zoneService.DeleteZoneAsync(id);
            if (!result)
                return NotFound(new { message = "Zone not found" });

            return NoContent();
        }

        // ==================== METODAT E REJA PËR LIDHJE ====================

        /// <summary>
        /// Lidh një location ekzistues me një zone
        /// </summary>
        [HttpPost("{zoneId}/locations/{locationId}")]
        public async Task<IActionResult> AddLocationToZone(int zoneId, int locationId)
        {
            // Kontrollo nëse zone ekziston
            var zone = await _context.Zones.FindAsync(zoneId);
            if (zone == null)
                return NotFound(new { message = "Zone not found" });

            // Kontrollo nëse location ekziston
            var location = await _context.Locations.FindAsync(locationId);
            if (location == null)
                return NotFound(new { message = "Location not found" });

            // Kontrollo nëse lidhja ekziston tashmë
            var existing = await _context.ZoneLocations
                .FirstOrDefaultAsync(zl => zl.ZoneId == zoneId && zl.LocationId == locationId);
            
            if (existing != null)
                return BadRequest(new { message = "Location already linked to this zone" });

            // Krijo lidhjen
            var zoneLocation = new ZoneLocation
            {
                ZoneId = zoneId,
                LocationId = locationId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ZoneLocations.Add(zoneLocation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Location linked to zone successfully" });
        }

        /// <summary>
        /// Shiko të gjitha location-et e një zone
        /// </summary>
        [HttpGet("{zoneId}/locations")]
        public async Task<IActionResult> GetZoneLocations(int zoneId)
        {
            var zone = await _context.Zones.FindAsync(zoneId);
            if (zone == null)
                return NotFound(new { message = "Zone not found" });

            var locations = await _context.ZoneLocations
                .Where(zl => zl.ZoneId == zoneId)
                .Include(zl => zl.Location)
                .Select(zl => zl.Location)
                .ToListAsync();

            return Ok(locations);
        }

        /// <summary>
        /// Hiq një location nga zona
        /// </summary>
        [HttpDelete("{zoneId}/locations/{locationId}")]
        public async Task<IActionResult> RemoveLocationFromZone(int zoneId, int locationId)
        {
            var zoneLocation = await _context.ZoneLocations
                .FirstOrDefaultAsync(zl => zl.ZoneId == zoneId && zl.LocationId == locationId);

            if (zoneLocation == null)
                return NotFound(new { message = "Location not found in this zone" });

            _context.ZoneLocations.Remove(zoneLocation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Location removed from zone successfully" });
        }
    }
}