using System.ComponentModel.DataAnnotations;

namespace FloodSystem.API.DTOs.Auth
{
    public class AssignRoleDto
    {
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}