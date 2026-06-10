using FloodSystem.API.Data;
using FloodSystem.API.Models.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace FloodSystem.API.Repositories.Dashboard;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuditLog>> GetAllAuditLogsAsync()
        => await _context.AuditLogs.OrderByDescending(a => a.CreatedAt).ToListAsync();

    public async Task<AuditLog> CreateAuditLogAsync(AuditLog log)
    {
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<List<Notification>> GetNotificationsByUserAsync(int userId)
        => await _context.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).ToListAsync();

    public async Task<Notification> CreateNotificationAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task MarkNotificationAsReadAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Setting>> GetAllSettingsAsync()
        => await _context.Settings.ToListAsync();

    public async Task UpdateSettingAsync(int id, string value)
    {
        var setting = await _context.Settings.FindAsync(id);
        if (setting != null)
        {
            setting.Value = value;
            setting.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Export> CreateExportAsync(Export export)
    {
        _context.Exports.Add(export);
        await _context.SaveChangesAsync();
        return export;
    }

    public async Task<Import> CreateImportAsync(Import import)
    {
        _context.Imports.Add(import);
        await _context.SaveChangesAsync();
        return import;
    }

    public async Task<ReportLog> CreateReportLogAsync(ReportLog log)
    {
        _context.ReportLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }
    public async Task<List<Notification>> GetNotificationsForUserAsync(int userId, bool includeAdminNotifications)
        => await _context.Notifications
            .Where(n => n.UserId == userId || (includeAdminNotifications && n.UserId == null))
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();
}