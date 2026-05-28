using System.ComponentModel.DataAnnotations;

namespace FloodSystem.API.DTOs.Weather
{
    public class CreateLocationDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }
    }
}