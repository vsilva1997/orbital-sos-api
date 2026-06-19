using OrbitalSOS.Application.Common;
using OrbitalSOS.Application.DTOs.ClimateAlerts;
using OrbitalSOS.Application.Interfaces;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Repositories;
namespace OrbitalSOS.Application.Services;
public class ClimateAlertService : IClimateAlertService
{
    private readonly ClimateAlertRepository _repo;
    public ClimateAlertService(ClimateAlertRepository repo) => _repo = repo;
    public async Task<PagedResult<ClimateAlertDto>> GetAllAsync(int page, int pageSize, string? region, bool? isActive)
    {
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, region, isActive);
        return new PagedResult<ClimateAlertDto>
        {
            Items = items.Select(MapDto),
            TotalCount = total, Page = page, PageSize = pageSize
        };
    }
    public async Task<ClimateAlertDto?> GetByIdAsync(Guid id)
    {
        var a = await _repo.GetByIdAsync(id);
        return a is null ? null : MapDto(a);
    }
    public async Task<ClimateAlertDto> CreateAsync(CreateClimateAlertDto dto)
    {
        var a = new ClimateAlert
        {
            RiskType = dto.RiskType, Level = dto.Level, AffectedRegion = dto.AffectedRegion,
            ExpectedAt = dto.ExpectedAt, Description = dto.Description
        };
        await _repo.AddAsync(a);
        return MapDto(a);
    }
    private static ClimateAlertDto MapDto(ClimateAlert a) => new()
    {
        Id = a.Id, RiskType = a.RiskType, Level = a.Level, AffectedRegion = a.AffectedRegion,
        ExpectedAt = a.ExpectedAt, Description = a.Description, IsActive = a.IsActive, CreatedAt = a.CreatedAt
    };
}
