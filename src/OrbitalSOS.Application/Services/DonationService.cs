using OrbitalSOS.Application.Common;
using OrbitalSOS.Application.DTOs.Donations;
using OrbitalSOS.Application.Interfaces;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Repositories;
namespace OrbitalSOS.Application.Services;
public class DonationService : IDonationService
{
    private readonly DonationRepository _repo;
    public DonationService(DonationRepository repo) => _repo = repo;
    public async Task<PagedResult<DonationDto>> GetAllAsync(int page, int pageSize)
    {
        var (items, total) = await _repo.GetPagedAsync(page, pageSize);
        return new PagedResult<DonationDto>
        {
            Items = items.Select(d => new DonationDto
            {
                Id = d.Id, ShelterId = d.ShelterId, ShelterName = d.Shelter?.Name ?? "",
                DonorName = d.DonorName, ItemType = d.ItemType, Quantity = d.Quantity, DonatedAt = d.DonatedAt
            }),
            TotalCount = total, Page = page, PageSize = pageSize
        };
    }
    public async Task<DonationDto> CreateAsync(CreateDonationDto dto)
    {
        var shelter = await _repo.GetShelterAsync(dto.ShelterId) ?? throw new KeyNotFoundException("Shelter not found.");
        var d = new Donation { ShelterId = dto.ShelterId, DonorName = dto.DonorName, ItemType = dto.ItemType, Quantity = dto.Quantity };
        await _repo.AddAsync(d);
        return new DonationDto { Id = d.Id, ShelterId = d.ShelterId, ShelterName = shelter.Name, DonorName = d.DonorName, ItemType = d.ItemType, Quantity = d.Quantity, DonatedAt = d.DonatedAt };
    }
    public async Task<ShelterBalanceDto> GetShelterBalanceAsync(Guid shelterId)
    {
        var shelter = await _repo.GetShelterAsync(shelterId) ?? throw new KeyNotFoundException("Shelter not found.");
        var donations = await _repo.GetDonationsByShelterId(shelterId);
        var needs = await _repo.GetNeedsByShelterId(shelterId);
        var allItems = needs.Select(n => n.ItemType).Union(donations.Select(d => d.ItemType)).Distinct();
        var balance = allItems.Select(item => new ItemBalanceDto
        {
            ItemType = item,
            QuantityNeeded = needs.Where(n => n.ItemType == item).Sum(n => n.QuantityNeeded),
            QuantityDonated = donations.Where(d => d.ItemType == item).Sum(d => d.Quantity)
        });
        return new ShelterBalanceDto { ShelterId = shelterId, ShelterName = shelter.Name, Items = balance };
    }
    public async Task<DonationDto> CreateShelterNeedAsync(CreateShelterNeedDto dto)
    {
        var shelter = await _repo.GetShelterAsync(dto.ShelterId) ?? throw new KeyNotFoundException("Shelter not found.");
        var n = new ShelterNeed { ShelterId = dto.ShelterId, ItemType = dto.ItemType, QuantityNeeded = dto.QuantityNeeded };
        await _repo.AddNeedAsync(n);
        return new DonationDto { Id = n.Id, ShelterId = n.ShelterId, ShelterName = shelter.Name, ItemType = n.ItemType, Quantity = n.QuantityNeeded };
    }
}
