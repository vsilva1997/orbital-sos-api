using Microsoft.EntityFrameworkCore;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Data;
namespace OrbitalSOS.Infrastructure.Repositories;
public class RiskAreaRepository
{
    private readonly AppDbContext _ctx;
    public RiskAreaRepository(AppDbContext ctx) => _ctx = ctx;
    public async Task<(List<RiskArea> items, int total)> GetPagedAsync(int page, int size, Guid? alertId)
    {
        var q = _ctx.RiskAreas.AsNoTracking().Include(r => r.ClimateAlert);
        var q2 = alertId.HasValue ? q.Where(r => r.ClimateAlertId == alertId.Value) : q;
        var total = await q2.CountAsync();
        var items = await q2.OrderBy(r => r.Name).Skip((page - 1) * size).Take(size).ToListAsync();
        return (items, total);
    }
    public async Task AddAsync(RiskArea r) { await _ctx.RiskAreas.AddAsync(r); await _ctx.SaveChangesAsync(); }
    public async Task<bool> AlertExistsAsync(Guid alertId) => await _ctx.ClimateAlerts.AnyAsync(a => a.Id == alertId);
}
