namespace OrbitalSOS.Application.DTOs.Donations;
public class DonationDto
{
    public Guid Id { get; set; }
    public Guid ShelterId { get; set; }
    public string ShelterName { get; set; } = string.Empty;
    public string DonorName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime DonatedAt { get; set; }
}
