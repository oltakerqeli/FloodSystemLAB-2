using System.ComponentModel.DataAnnotations;

namespace FloodSystem.API.Models.Weather
{
    public class WeatherData
    {
        public int Id { get; set; }
        public int LocationId { get; set; }

        [Required]
        public decimal Temperature { get; set; }

        [Required]
        public decimal Rainfall { get; set; }

        [Required]
        public decimal Humidity { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Location Location { get; set; } = null!;
    }
}