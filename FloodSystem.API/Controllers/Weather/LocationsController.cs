using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FloodSystem.API.DTOs.Weather;
using FloodSystem.API.Services.Weather;
using FloodSystem.API.Data;
using Microsoft.EntityFrameworkCore;
using FloodSystem.API.Models.Weather;

namespace FloodSystem.API.Controllers.Weather
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly LocationService _locationService;
        private readonly ApplicationDbContext _context;  // ✅ Shto këtë

        public LocationsController(LocationService locationService, ApplicationDbContext context)  // ✅ Shto context
        {
            _locationService = locationService;
            _context = context;  // ✅ Shto këtë
        }

       [HttpGet]
[Authorize]
public async Task<IActionResult> GetAll()
{
    // Kontrollo nëse përdoruesi është Admin ose Authority
    var isAdminOrAuthority = User.IsInRole("Admin") || User.IsInRole("Authority");
    
    // Nëse është Admin/Authority, merr TË GJITHA lokacionet
    // Përndryshe, merr vetëm ato aktive
    var locations = await _locationService.GetAllLocationsAsync(isAdminOrAuthority);
    return Ok(locations);
}

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var location = await _locationService.GetLocationByIdAsync(id);
            if (location == null)
                return NotFound(new { message = "Location not found" });

            return Ok(location);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Authority")]
        public async Task<IActionResult> Create([FromBody] CreateLocationDto dto)
        {
            try
            {
                var location = await _locationService.CreateLocationAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Authority")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateLocationDto dto)
        {
            var location = await _locationService.UpdateLocationAsync(id, dto);
            if (location == null)
                return NotFound(new { message = "Location not found" });

            return Ok(location);
        }

     [HttpDelete("{id}")]
[Authorize(Roles = "Admin,Authority")]  // Authority gjithashtu mund të fshijë
public async Task<IActionResult> Delete(int id)
{
    var result = await _locationService.DeleteLocationAsync(id);
    if (!result)
        return NotFound(new { message = "Location not found" });

    return NoContent();
}

        /// <summary>
        /// Shiko të gjitha zonat ku bën pjesë një location
        /// </summary>
        [HttpGet("{id}/zones")]
        public async Task<IActionResult> GetLocationZones(int id)
        {
            // Kontrollo nëse location ekziston
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
                return NotFound(new { message = "Location not found" });

            // Merr të gjitha zonat e këtij location-i
            var zones = await _context.ZoneLocations
                .Where(zl => zl.LocationId == id)
                .Include(zl => zl.Zone)
                .Select(zl => zl.Zone)
                .ToListAsync();

            return Ok(zones);
        }
    }
}