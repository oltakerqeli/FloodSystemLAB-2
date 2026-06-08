using FloodSystem.API.DTOs.Dashboard;

namespace FloodSystem.API.Services.Dashboard;

public interface IDashboardService
{
    Task<List<AuditLogDto>> GetAllAuditLogsAsync();
    Task<List<NotificationDto>> GetNotificationsByUserAsync(int userId);
    Task CreateNotificationAsync(CreateNotificationDto dto);
    Task MarkNotificationAsReadAsync(int id);
    Task<List<SettingDto>> GetAllSettingsAsync();
    Task UpdateSettingAsync(int id, string value);
    Task<ExportFileResult> ExportReportsAsync(CreateExportDto dto, int userId);
    Task<ImportDto> ImportDataAsync(IFormFile file, string type, int userId);
}