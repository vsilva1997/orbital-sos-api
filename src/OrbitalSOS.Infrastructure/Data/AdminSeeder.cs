using Microsoft.EntityFrameworkCore;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Infrastructure.Data;
// Cria o administrador inicial a partir das variáveis Admin__Email / Admin__Password (opcionais)
public static class AdminSeeder
{
    public static async Task<bool> SeedAsync(AppDbContext db, string? email, string? password, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return false;
        if (await db.Users.AnyAsync(u => u.Email == email)) return false;
        db.Users.Add(new User
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Administrador" : name,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.Admin
        });
        await db.SaveChangesAsync();
        return true;
    }
}
