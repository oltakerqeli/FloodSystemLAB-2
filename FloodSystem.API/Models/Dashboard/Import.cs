using FloodSystem.API.Models.Auth;

namespace FloodSystem.API.Models.Dashboard;

public class Import
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}