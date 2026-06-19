namespace OrbitalSOS.Domain.Entities;
public class ShelterNeed
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ShelterId { get; set; }
    public Shelter Shelter { get; set; } = null!;
    public string ItemType { get; set; } = string.Empty;
    public int QuantityNeeded { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
