using Microsoft.EntityFrameworkCore;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Data;
namespace OrbitalSOS.Infrastructure.Repositories;
public class ClimateAlertRepository
{
    private readonly AppDbContext _ctx;
    public ClimateAlertRepository(AppDbContext ctx) => _ctx = ctx;
    public async Task<(List<ClimateAlert> items, int total)> GetPagedAsync(int page, int size, string? region, bool? isActive)
    {
        var q = _ctx.ClimateAlerts.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(region)) q = q.Where(a => a.AffectedRegion.Contains(region));
        if (isActive.HasValue) q = q.Where(a => a.IsActive == isActive.Value);
        var total = await q.CountAsync();
        var items = await q.OrderByDescending(a => a.CreatedAt).Skip((page - 1) * size).Take(size).ToListAsync();
        return (items, total);
    }
    public async Task<ClimateAlert?> GetByIdAsync(Guid id) => await _ctx.ClimateAlerts.FindAsync(id);
    public async Task AddAsync(ClimateAlert a) { await _ctx.ClimateAlerts.AddAsync(a); await _ctx.SaveChangesAsync(); }
}
