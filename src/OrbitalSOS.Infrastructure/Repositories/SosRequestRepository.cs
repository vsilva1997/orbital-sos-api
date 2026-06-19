using Microsoft.EntityFrameworkCore;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Data;
namespace OrbitalSOS.Infrastructure.Repositories;
public class SosRequestRepository
{
    private readonly AppDbContext _ctx;
    public SosRequestRepository(AppDbContext ctx) => _ctx = ctx;
    public async Task<(List<SosRequest> items, int total)> GetPagedAsync(int page, int size)
    {
        var q = _ctx.SosRequests.AsNoTracking().Include(r => r.User).Include(r => r.SuggestedShelter);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(r => r.CreatedAt).Skip((page - 1) * size).Take(size).ToListAsync();
        return (items, total);
    }
    public async Task<SosRequest?> GetByIdAsync(Guid id) => await _ctx.SosRequests.Include(r => r.User).Include(r => r.SuggestedShelter).FirstOrDefaultAsync(r => r.Id == id);
    public async Task AddAsync(SosRequest r) { await _ctx.SosRequests.AddAsync(r); await _ctx.SaveChangesAsync(); }
    public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();
}
