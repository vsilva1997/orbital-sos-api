using OrbitalSOS.Application.Common;
using OrbitalSOS.Application.DTOs.SosRequests;
namespace OrbitalSOS.Application.Interfaces;
public interface ISosRequestService
{
    Task<PagedResult<SosRequestDto>> GetAllAsync(int page, int pageSize);
    Task<SosRequestDto?> GetByIdAsync(Guid id);
    Task<SosRequestDto> CreateAsync(CreateSosRequestDto dto, Guid userId);
    Task<SosRequestDto> UpdateStatusAsync(Guid id, UpdateSosStatusDto dto);
}
