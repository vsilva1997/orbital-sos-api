using OrbitalSOS.Application.Common;
using OrbitalSOS.Application.DTOs.SosRequests;
using OrbitalSOS.Application.Interfaces;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Repositories;
namespace OrbitalSOS.Application.Services;
public class SosRequestService : ISosRequestService
{
    private readonly SosRequestRepository _repo;
    public SosRequestService(SosRequestRepository repo) => _repo = repo;
    public async Task<PagedResult<SosRequestDto>> GetAllAsync(int page, int pageSize)
    {
        var (items, total) = await _repo.GetPagedAsync(page, pageSize);
        return new PagedResult<SosRequestDto>
        {
            Items = items.Select(MapDto),
            TotalCount = total, Page = page, PageSize = pageSize
        };
    }
    public async Task<SosRequestDto?> GetByIdAsync(Guid id)
    {
        var r = await _repo.GetByIdAsync(id);
        return r is null ? null : MapDto(r);
    }
    public async Task<SosRequestDto> CreateAsync(CreateSosRequestDto dto, Guid userId)
    {
        var r = new SosRequest
        {
            UserId = userId, EmergencyType = dto.EmergencyType,
            Latitude = dto.Latitude, Longitude = dto.Longitude,
            SuggestedShelterId = dto.SuggestedShelterId, Notes = dto.Notes
        };
        await _repo.AddAsync(r);
        return new SosRequestDto
        {
            Id = r.Id, UserId = r.UserId, EmergencyType = r.EmergencyType,
            Latitude = r.Latitude, Longitude = r.Longitude,
            SuggestedShelterId = r.SuggestedShelterId, Status = r.Status,
            Notes = r.Notes, CreatedAt = r.CreatedAt, UpdatedAt = r.UpdatedAt
        };
    }
    public async Task<SosRequestDto> UpdateStatusAsync(Guid id, UpdateSosStatusDto dto)
    {
        var r = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("SosRequest not found.");
        r.Status = dto.Status;
        r.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync();
        return MapDto(r);
    }
    private static SosRequestDto MapDto(SosRequest r) => new()
    {
        Id = r.Id, UserId = r.UserId, UserName = r.User?.Name ?? "",
        SuggestedShelterId = r.SuggestedShelterId, SuggestedShelterName = r.SuggestedShelter?.Name,
        EmergencyType = r.EmergencyType, Latitude = r.Latitude, Longitude = r.Longitude,
        Status = r.Status, Notes = r.Notes, CreatedAt = r.CreatedAt, UpdatedAt = r.UpdatedAt
    };
}
