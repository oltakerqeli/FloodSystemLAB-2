using FloodSystem.API.Models.Auth;
using FloodSystem.API.Models.Weather;
namespace FloodSystem.API.Models.Reporting;

public class DrainReport
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LocationId { get; set; }
    public int? FileId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string? ReporterName { get; set; }
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ReportStatus Status { get; set; } = null!;
    public AppFile? File { get; set; }
    public Location Location { get; set; } = null!;
}