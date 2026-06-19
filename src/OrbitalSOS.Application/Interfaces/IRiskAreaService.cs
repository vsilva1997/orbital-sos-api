using OrbitalSOS.Application.Common;
using OrbitalSOS.Application.DTOs.RiskAreas;
namespace OrbitalSOS.Application.Interfaces;
public interface IRiskAreaService
{
    Task<PagedResult<RiskAreaDto>> GetAllAsync(int page, int pageSize, Guid? alertId);
    Task<RiskAreaDto> CreateAsync(CreateRiskAreaDto dto);
}
