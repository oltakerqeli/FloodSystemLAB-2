namespace FloodSystem.API.Models.Reporting;

public class ReportStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<FloodReport> FloodReports { get; set; } = new List<FloodReport>();
    public ICollection<DrainReport> DrainReports { get; set; } = new List<DrainReport>();
}