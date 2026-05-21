using FloodSystem.API.Models.Auth;
namespace FloodSystem.API.Models.Reporting;

public class UserReport
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ReportId { get; set; }
    public string ReportType { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}