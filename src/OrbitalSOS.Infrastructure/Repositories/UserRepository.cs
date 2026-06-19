using Microsoft.EntityFrameworkCore;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Infrastructure.Data;
namespace OrbitalSOS.Infrastructure.Repositories;
public class UserRepository
{
    private readonly AppDbContext _ctx;
    public UserRepository(AppDbContext ctx) => _ctx = ctx;
    public async Task<User?> GetByEmailAsync(string email) => await _ctx.Users.FirstOrDefaultAsync(u => u.Email == email);
    public async Task<bool> EmailExistsAsync(string email) => await _ctx.Users.AnyAsync(u => u.Email == email);
    public async Task AddAsync(User u) { await _ctx.Users.AddAsync(u); await _ctx.SaveChangesAsync(); }
}
