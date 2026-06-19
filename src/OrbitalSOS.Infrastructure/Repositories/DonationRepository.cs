using Microsoft.EntityFrameworkCore;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Data;
namespace OrbitalSOS.Infrastructure.Repositories;
public class DonationRepository
{
    private readonly AppDbContext _ctx;
    public DonationRepository(AppDbContext ctx) => _ctx = ctx;
    public async Task<(List<Donation> items, int total)> GetPagedAsync(int page, int size)
    {
        var q = _ctx.Donations.AsNoTracking().Include(d => d.Shelter);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(d => d.DonatedAt).Skip((page - 1) * size).Take(size).ToListAsync();
        return (items, total);
    }
    public async Task AddAsync(Donation d) { await _ctx.Donations.AddAsync(d); await _ctx.SaveChangesAsync(); }
    public async Task AddNeedAsync(ShelterNeed n) { await _ctx.ShelterNeeds.AddAsync(n); await _ctx.SaveChangesAsync(); }
    public async Task<Shelter?> GetShelterAsync(Guid id) => await _ctx.Shelters.FindAsync(id);
    public async Task<List<Donation>> GetDonationsByShelterId(Guid shelterId) => await _ctx.Donations.AsNoTracking().Where(d => d.ShelterId == shelterId).ToListAsync();
    public async Task<List<ShelterNeed>> GetNeedsByShelterId(Guid shelterId) => await _ctx.ShelterNeeds.AsNoTracking().Where(n => n.ShelterId == shelterId).ToListAsync();
}
