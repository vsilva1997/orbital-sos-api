namespace OrbitalSOS.Domain.Entities;
public class Donation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ShelterId { get; set; }
    public Shelter Shelter { get; set; } = null!;
    public string DonorName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime DonatedAt { get; set; } = DateTime.UtcNow;
}
