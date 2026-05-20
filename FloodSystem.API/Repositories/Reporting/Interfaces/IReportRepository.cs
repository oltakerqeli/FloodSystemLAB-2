using FloodSystem.API.Models.Reporting;
namespace FloodSystem.API.Repositories.Reporting;

public interface IReportRepository
{
    Task<List<FloodReport>> GetAllFloodReportsAsync();
    Task<FloodReport?> GetFloodReportByIdAsync(int id);
    Task<FloodReport> CreateFloodReportAsync(FloodReport report);
    Task<DrainReport> CreateDrainReportAsync(DrainReport report);
    Task<List<DrainReport>> GetAllDrainReportsAsync();
    Task UpdateReportStatusAsync(int id, int statusId, string type);
}