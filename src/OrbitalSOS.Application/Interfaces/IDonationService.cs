using OrbitalSOS.Application.Common;
using OrbitalSOS.Application.DTOs.Donations;
namespace OrbitalSOS.Application.Interfaces;
public interface IDonationService
{
    Task<PagedResult<DonationDto>> GetAllAsync(int page, int pageSize);
    Task<DonationDto> CreateAsync(CreateDonationDto dto);
    Task<ShelterBalanceDto> GetShelterBalanceAsync(Guid shelterId);
    Task<DonationDto> CreateShelterNeedAsync(CreateShelterNeedDto dto);
}
