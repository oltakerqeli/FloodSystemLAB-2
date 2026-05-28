using System.ComponentModel.DataAnnotations;

namespace FloodSystem.API.Models.Weather
{
    public class Location
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<ZoneLocation> ZoneLocations { get; set; } = new List<ZoneLocation>();
        public ICollection<WeatherData> WeatherData { get; set; } = new List<WeatherData>();
        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
        public ICollection<TrafficUpdate> TrafficUpdates { get; set; } = new List<TrafficUpdate>();
    }
}