using FloodSystem.API.DTOs.Dashboard;
using FloodSystem.API.Models.Dashboard;
using FloodSystem.API.Repositories.Dashboard;

namespace FloodSystem.API.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repo;

    public DashboardService(IDashboardRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<AuditLogDto>> GetAllAuditLogsAsync()
    {
        var logs = await _repo.GetAllAuditLogsAsync();
        return logs.Select(l => new AuditLogDto
        {
            Id = l.Id,
            UserId = l.UserId,
            Action = l.Action,
            Entity = l.Entity,
            EntityId = l.EntityId,
            OldValue = l.OldValue,
            NewValue = l.NewValue,
            IpAddress = l.IpAddress,
            CreatedAt = l.CreatedAt
        }).ToList();
    }

    public async Task<List<NotificationDto>> GetNotificationsByUserAsync(int userId)
    {
        var notifications = await _repo.GetNotificationsByUserAsync(userId);
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Type = n.Type,
            Title = n.Title,
            Message = n.Message,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    public async Task CreateNotificationAsync(CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            UserId = dto.UserId,
            Type = dto.Type,
            Title = dto.Title,
            Message = dto.Message
        };
        await _repo.CreateNotificationAsync(notification);
    }

    public async Task MarkNotificationAsReadAsync(int id)
        => await _repo.MarkNotificationAsReadAsync(id);

    public async Task<List<SettingDto>> GetAllSettingsAsync()
    {
        var settings = await _repo.GetAllSettingsAsync();
        return settings.Select(s => new SettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = s.Value,
            Description = s.Description
        }).ToList();
    }

    public async Task UpdateSettingAsync(int id, string value)
        => await _repo.UpdateSettingAsync(id, value);

    public async Task<ExportDto> CreateExportAsync(CreateExportDto dto, int userId)
    {
        var export = new Export
        {
            UserId = userId,
            Type = dto.Type,
            Format = dto.Format
        };
        var created = await _repo.CreateExportAsync(export);
        return new ExportDto { Id = created.Id, Type = created.Type, Format = created.Format, CreatedAt = created.CreatedAt };
    }

    public async Task<ImportDto> CreateImportAsync(CreateImportDto dto, int userId)
    {
        var import = new Import
        {
            UserId = userId,
            Type = dto.Type,
            Format = dto.Format
        };
        var created = await _repo.CreateImportAsync(import);
        return new ImportDto { Id = created.Id, Type = created.Type, Format = created.Format, CreatedAt = created.CreatedAt };
    }
}