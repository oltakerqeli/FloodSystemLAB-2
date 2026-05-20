using FloodSystem.API.Models.Auth;
namespace FloodSystem.API.Models.Reporting;

public class DrainReport
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LocationId { get; set; }
    public int? FileId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ReportStatus Status { get; set; } = null!;
    public AppFile? File { get; set; }
}