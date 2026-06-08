using FloodSystem.API.DTOs.Reporting;

namespace FloodSystem.API.Services.Reporting;

public interface IReportService
{
    Task<ReportResponseDto> CreateFloodReportAsync(CreateFloodReportDto dto, int userId);
    Task<ReportResponseDto> CreateDrainReportAsync(CreateDrainReportDto dto, int userId);
    Task<List<ReportResponseDto>> GetAllFloodReportsAsync(int userId);
    Task<List<ReportResponseDto>> GetAllDrainReportsAsync(int userId);
    Task UpdateReportStatusAsync(int id, int statusId, string type);
}