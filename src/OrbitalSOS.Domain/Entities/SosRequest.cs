using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Domain.Entities;
public class SosRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid? SuggestedShelterId { get; set; }
    public Shelter? SuggestedShelter { get; set; }
    public EmergencyType EmergencyType { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public SosStatus Status { get; set; } = SosStatus.Pendente;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
