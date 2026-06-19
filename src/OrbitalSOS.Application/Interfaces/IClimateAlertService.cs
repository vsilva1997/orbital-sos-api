using OrbitalSOS.Application.Common;
using OrbitalSOS.Application.DTOs.ClimateAlerts;
namespace OrbitalSOS.Application.Interfaces;
public interface IClimateAlertService
{
    Task<PagedResult<ClimateAlertDto>> GetAllAsync(int page, int pageSize, string? region, bool? isActive);
    Task<ClimateAlertDto?> GetByIdAsync(Guid id);
    Task<ClimateAlertDto> CreateAsync(CreateClimateAlertDto dto);
}
