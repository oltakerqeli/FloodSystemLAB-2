using System.ComponentModel.DataAnnotations;

namespace FloodSystem.API.DTOs.Weather
{
    public class CreateZoneDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal CriticalRainfallThreshold { get; set; } = 10m;
    }
}