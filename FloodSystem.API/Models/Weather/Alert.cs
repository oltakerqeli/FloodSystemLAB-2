using System.ComponentModel.DataAnnotations;

namespace FloodSystem.API.Models.Weather
{
    public class Alert
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string RiskLevel { get; set; } = string.Empty;

        public int LocationId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Location Location { get; set; } = null!;
    }
}