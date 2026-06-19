using Microsoft.EntityFrameworkCore;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Data;
namespace OrbitalSOS.Infrastructure.Repositories;
public class ShelterRepository
{
    private readonly AppDbContext _ctx;
    public ShelterRepository(AppDbContext ctx) => _ctx = ctx;
    public async Task<(List<Shelter> items, int total)> GetPagedAsync(int page, int size, double? lat, double? lon, double? radiusKm)
    {
        var q = _ctx.Shelters.AsNoTracking().Where(s => s.IsActive);
        if (lat.HasValue && lon.HasValue && radiusKm.HasValue)
        {
            // Haversine approx: 1 degree lat ≈ 111 km
            double dlat = radiusKm.Value / 111.0;
            double dlon = radiusKm.Value / (111.0 * Math.Cos(lat.Value * Math.PI / 180.0));
            q = q.Where(s => Math.Abs(s.Latitude - lat.Value) <= dlat && Math.Abs(s.Longitude - lon.Value) <= dlon);
        }
        var total = await q.CountAsync();
        var items = await q.OrderBy(s => s.Name).Skip((page - 1) * size).Take(size).ToListAsync();
        return (items, total);
    }
    public async Task<Shelter?> GetByIdAsync(Guid id) => await _ctx.Shelters.FindAsync(id);
    public async Task AddAsync(Shelter s) { await _ctx.Shelters.AddAsync(s); await _ctx.SaveChangesAsync(); }
}
