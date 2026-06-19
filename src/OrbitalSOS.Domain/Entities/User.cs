using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Domain.Entities;
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Usuario;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<SosRequest> SosRequests { get; set; } = new List<SosRequest>();
}
