using System.ComponentModel.DataAnnotations;

namespace FloodSystem.API.Models.Weather
{
    public class Zone
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal CriticalRainfallThreshold { get; set; } = 10m;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<ZoneLocation> ZoneLocations { get; set; } = new List<ZoneLocation>();
    }
}