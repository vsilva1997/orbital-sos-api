using System.ComponentModel.DataAnnotations;
namespace OrbitalSOS.Application.DTOs.Donations;
public class CreateDonationDto
{
    [Required] public Guid ShelterId { get; set; }
    [Required] public string DonorName { get; set; } = string.Empty;
    [Required] public string ItemType { get; set; } = string.Empty;
    [Range(1, int.MaxValue)] public int Quantity { get; set; }
}
