using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Domain.Entities;
public class Shelter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Capacity { get; set; }
    public int CurrentOccupancy { get; set; }
    public ShelterType Type { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    public ICollection<ShelterNeed> ShelterNeeds { get; set; } = new List<ShelterNeed>();
    public ICollection<SosRequest> SosRequests { get; set; } = new List<SosRequest>();
}
