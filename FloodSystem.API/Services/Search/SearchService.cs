using Microsoft.EntityFrameworkCore;
using FloodSystem.API.Data;
using FloodSystem.API.DTOs.Weather;
using FloodSystem.API.DTOs.Search;
using FloodSystem.API.Models.Weather;

namespace FloodSystem.API.Services.Search
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== SEARCH ALERTS ====================
        public async Task<SearchResultDto<AlertDto>> SearchAlertsAsync(AlertSearchDto dto)
        {
            var query = _context.Alerts.Include(a => a.Location).AsQueryable();

            if (dto.LocationId.HasValue)
                query = query.Where(a => a.LocationId == dto.LocationId);
            if (!string.IsNullOrEmpty(dto.RiskLevel))
                query = query.Where(a => a.RiskLevel == dto.RiskLevel);
            if (dto.FromDate.HasValue)
                query = query.Where(a => a.CreatedAt >= dto.FromDate);
            if (dto.ToDate.HasValue)
                query = query.Where(a => a.CreatedAt <= dto.ToDate);
            if (!string.IsNullOrEmpty(dto.SearchTerm))
                query = query.Where(a => a.Message.Contains(dto.SearchTerm));

            query = dto.SortBy?.ToLower() switch
            {
                "risklevel" => dto.SortDescending ? query.OrderByDescending(a => a.RiskLevel) : query.OrderBy(a => a.RiskLevel),
                _ => dto.SortDescending ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(a => new AlertDto
                {
                    Id = a.Id,
                    Type = a.Type,
                    Message = a.Message,
                    RiskLevel = a.RiskLevel,
                    LocationId = a.LocationId,
                    LocationName = a.Location != null ? a.Location.Name : "",
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return new SearchResultDto<AlertDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = dto.Page,
                PageSize = dto.PageSize
            };
        }

        // ==================== SEARCH LOCATIONS ====================
        public async Task<SearchResultDto<LocationDto>> SearchLocationsAsync(LocationSearchDto dto)
        {
            var query = _context.Locations.AsQueryable();

            if (!string.IsNullOrEmpty(dto.Name))
                query = query.Where(l => l.Name.Contains(dto.Name));
            if (dto.IsActive.HasValue)
                query = query.Where(l => l.IsActive == dto.IsActive);
            if (!string.IsNullOrEmpty(dto.SearchTerm))
                query = query.Where(l => l.Name.Contains(dto.SearchTerm) || (l.Description != null && l.Description.Contains(dto.SearchTerm)));

            query = dto.SortBy?.ToLower() switch
            {
                "name" => dto.SortDescending ? query.OrderByDescending(l => l.Name) : query.OrderBy(l => l.Name),
                _ => dto.SortDescending ? query.OrderByDescending(l => l.CreatedAt) : query.OrderBy(l => l.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Description = l.Description,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude,
                    IsActive = l.IsActive,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();

            return new SearchResultDto<LocationDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = dto.Page,
                PageSize = dto.PageSize
            };
        }

        // ==================== SEARCH WEATHER DATA ====================
        public async Task<SearchResultDto<WeatherDataDto>> SearchWeatherDataAsync(WeatherDataSearchDto dto)
        {
            var query = _context.WeatherData.Include(w => w.Location).AsQueryable();

            if (dto.LocationId.HasValue)
                query = query.Where(w => w.LocationId == dto.LocationId);
            if (dto.MinRainfall.HasValue)
                query = query.Where(w => w.Rainfall >= dto.MinRainfall);
            if (dto.MaxRainfall.HasValue)
                query = query.Where(w => w.Rainfall <= dto.MaxRainfall);
            if (dto.FromDate.HasValue)
                query = query.Where(w => w.RecordedAt >= dto.FromDate);
            if (dto.ToDate.HasValue)
                query = query.Where(w => w.RecordedAt <= dto.ToDate);

            query = dto.SortBy?.ToLower() switch
            {
                "rainfall" => dto.SortDescending ? query.OrderByDescending(w => w.Rainfall) : query.OrderBy(w => w.Rainfall),
                "temperature" => dto.SortDescending ? query.OrderByDescending(w => w.Temperature) : query.OrderBy(w => w.Temperature),
                "humidity" => dto.SortDescending ? query.OrderByDescending(w => w.Humidity) : query.OrderBy(w => w.Humidity),
                _ => dto.SortDescending ? query.OrderByDescending(w => w.RecordedAt) : query.OrderBy(w => w.RecordedAt)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(w => new WeatherDataDto
                {
                    Id = w.Id,
                    LocationId = w.LocationId,
                    LocationName = w.Location != null ? w.Location.Name : "",
                    Temperature = w.Temperature,
                    Rainfall = w.Rainfall,
                    Humidity = w.Humidity,
                    RecordedAt = w.RecordedAt,
                    RiskLevel = GetRiskLevel(w.Rainfall)
                })
                .ToListAsync();

            return new SearchResultDto<WeatherDataDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = dto.Page,
                PageSize = dto.PageSize
            };
        }

        // ==================== SEARCH TRAFFIC UPDATES ====================
        public async Task<SearchResultDto<TrafficUpdateDto>> SearchTrafficUpdatesAsync(TrafficUpdateSearchDto dto)
        {
            var query = _context.TrafficUpdates.Include(t => t.Location).AsQueryable();

            if (dto.LocationId.HasValue)
                query = query.Where(t => t.LocationId == dto.LocationId);
            if (!string.IsNullOrEmpty(dto.Status))
                query = query.Where(t => t.Status == dto.Status);
            if (dto.FromDate.HasValue)
                query = query.Where(t => t.CreatedAt >= dto.FromDate);
            if (dto.ToDate.HasValue)
                query = query.Where(t => t.CreatedAt <= dto.ToDate);

            query = dto.SortBy?.ToLower() switch
            {
                "status" => dto.SortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
                _ => dto.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(t => new TrafficUpdateDto
                {
                    Id = t.Id,
                    LocationId = t.LocationId,
                    LocationName = t.Location != null ? t.Location.Name : "",
                    Status = t.Status,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return new SearchResultDto<TrafficUpdateDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = dto.Page,
                PageSize = dto.PageSize
            };
        }

        // ==================== SEARCH ZONES ====================
        public async Task<SearchResultDto<ZoneDto>> SearchZonesAsync(ZoneSearchDto dto)
        {
            var query = _context.Zones.AsQueryable();

            if (!string.IsNullOrEmpty(dto.Name))
                query = query.Where(z => z.Name.Contains(dto.Name));
            if (dto.IsActive.HasValue)
                query = query.Where(z => z.IsActive == dto.IsActive);
            if (!string.IsNullOrEmpty(dto.SearchTerm))
                query = query.Where(z => z.Name.Contains(dto.SearchTerm) || (z.Description != null && z.Description.Contains(dto.SearchTerm)));

            query = dto.SortBy?.ToLower() switch
            {
                "name" => dto.SortDescending ? query.OrderByDescending(z => z.Name) : query.OrderBy(z => z.Name),
                "criticalrainfallthreshold" => dto.SortDescending ? query.OrderByDescending(z => z.CriticalRainfallThreshold) : query.OrderBy(z => z.CriticalRainfallThreshold),
                _ => dto.SortDescending ? query.OrderByDescending(z => z.CreatedAt) : query.OrderBy(z => z.CreatedAt)
            };

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(z => new ZoneDto
                {
                    Id = z.Id,
                    Name = z.Name,
                    Description = z.Description,
                    CriticalRainfallThreshold = z.CriticalRainfallThreshold,
                    IsActive = z.IsActive,
                    CreatedAt = z.CreatedAt
                })
                .ToListAsync();

            return new SearchResultDto<ZoneDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = dto.Page,
                PageSize = dto.PageSize
            };
        }

        private static string GetRiskLevel(decimal rainfall)
        {
            if (rainfall > 10) return "HIGH";
            if (rainfall > 5) return "MEDIUM";
            return "LOW";
        }
    }
}