using OrbitalSOS.Application.Common;
using OrbitalSOS.Application.DTOs.Shelters;
namespace OrbitalSOS.Application.Interfaces;
public interface IShelterService
{
    Task<PagedResult<ShelterDto>> GetAllAsync(int page, int pageSize, double? lat, double? lon, double? radiusKm);
    Task<ShelterDto?> GetByIdAsync(Guid id);
    Task<ShelterDto> CreateAsync(CreateShelterDto dto);
}
