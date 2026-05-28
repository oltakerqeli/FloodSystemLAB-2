using System.ComponentModel.DataAnnotations;

namespace FloodSystem.API.Models.Weather
{
    public class TrafficUpdate
    {
        public int Id { get; set; }
        public int LocationId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Location Location { get; set; } = null!;
    }
}