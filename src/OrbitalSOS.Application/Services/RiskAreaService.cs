using OrbitalSOS.Application.Common;
using OrbitalSOS.Application.DTOs.RiskAreas;
using OrbitalSOS.Application.Interfaces;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Repositories;
namespace OrbitalSOS.Application.Services;
public class RiskAreaService : IRiskAreaService
{
    private readonly RiskAreaRepository _repo;
    public RiskAreaService(RiskAreaRepository repo) => _repo = repo;
    public async Task<PagedResult<RiskAreaDto>> GetAllAsync(int page, int pageSize, Guid? alertId)
    {
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, alertId);
        return new PagedResult<RiskAreaDto>
        {
            Items = items.Select(MapDto),
            TotalCount = total, Page = page, PageSize = pageSize
        };
    }
    public async Task<RiskAreaDto> CreateAsync(CreateRiskAreaDto dto)
    {
        if (!await _repo.AlertExistsAsync(dto.ClimateAlertId))
            throw new KeyNotFoundException("ClimateAlert not found.");
        var r = new RiskArea
        {
            ClimateAlertId = dto.ClimateAlertId, Name = dto.Name,
            PolygonGeoJson = dto.PolygonGeoJson, DangerLevel = dto.DangerLevel
        };
        await _repo.AddAsync(r);
        return new RiskAreaDto
        {
            Id = r.Id, ClimateAlertId = r.ClimateAlertId, Name = r.Name,
            PolygonGeoJson = r.PolygonGeoJson, DangerLevel = r.DangerLevel, CreatedAt = r.CreatedAt
        };
    }
    private static RiskAreaDto MapDto(RiskArea r) => new()
    {
        Id = r.Id, ClimateAlertId = r.ClimateAlertId,
        ClimateAlertRegion = r.ClimateAlert?.AffectedRegion ?? "",
        Name = r.Name, PolygonGeoJson = r.PolygonGeoJson,
        DangerLevel = r.DangerLevel, CreatedAt = r.CreatedAt
    };
}
