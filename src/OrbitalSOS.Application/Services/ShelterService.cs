using OrbitalSOS.Application.Common;
using OrbitalSOS.Application.DTOs.Shelters;
using OrbitalSOS.Application.Interfaces;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Repositories;
namespace OrbitalSOS.Application.Services;
public class ShelterService : IShelterService
{
    private readonly ShelterRepository _repo;
    public ShelterService(ShelterRepository repo) => _repo = repo;
    public async Task<PagedResult<ShelterDto>> GetAllAsync(int page, int pageSize, double? lat, double? lon, double? radiusKm)
    {
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, lat, lon, radiusKm);
        return new PagedResult<ShelterDto>
        {
            Items = items.Select(MapDto),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }
    public async Task<ShelterDto?> GetByIdAsync(Guid id)
    {
        var s = await _repo.GetByIdAsync(id);
        return s is null ? null : MapDto(s);
    }
    public async Task<ShelterDto> CreateAsync(CreateShelterDto dto)
    {
        var s = new Shelter
        {
            Name = dto.Name, Address = dto.Address, Latitude = dto.Latitude,
            Longitude = dto.Longitude, Capacity = dto.Capacity, Type = dto.Type
        };
        await _repo.AddAsync(s);
        return MapDto(s);
    }
    private static ShelterDto MapDto(Shelter s) => new()
    {
        Id = s.Id, Name = s.Name, Address = s.Address, Latitude = s.Latitude,
        Longitude = s.Longitude, Capacity = s.Capacity, CurrentOccupancy = s.CurrentOccupancy,
        Type = s.Type, IsActive = s.IsActive, CreatedAt = s.CreatedAt
    };
}
