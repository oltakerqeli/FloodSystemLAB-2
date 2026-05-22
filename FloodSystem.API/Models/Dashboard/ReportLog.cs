namespace FloodSystem.API.Models.Dashboard;

public class ReportLog
{
    public int Id { get; set; }
    public int ReportId { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}