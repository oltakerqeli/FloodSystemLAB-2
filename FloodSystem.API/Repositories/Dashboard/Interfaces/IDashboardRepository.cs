using FloodSystem.API.Models.Dashboard;

namespace FloodSystem.API.Repositories.Dashboard;

public interface IDashboardRepository
{
    Task<List<AuditLog>> GetAllAuditLogsAsync();
    Task<AuditLog> CreateAuditLogAsync(AuditLog log);
    Task<List<Notification>> GetNotificationsByUserAsync(int userId);
    Task<Notification> CreateNotificationAsync(Notification notification);
    Task MarkNotificationAsReadAsync(int id);
    Task<List<Setting>> GetAllSettingsAsync();
    Task UpdateSettingAsync(int id, string value);
    Task<Export> CreateExportAsync(Export export);
    Task<Import> CreateImportAsync(Import import);
    Task<ReportLog> CreateReportLogAsync(ReportLog log);
}