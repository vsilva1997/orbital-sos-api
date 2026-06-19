using System.ComponentModel.DataAnnotations;
namespace OrbitalSOS.Application.DTOs.Donations;
public class CreateShelterNeedDto
{
    [Required] public Guid ShelterId { get; set; }
    [Required] public string ItemType { get; set; } = string.Empty;
    [Range(1, int.MaxValue)] public int QuantityNeeded { get; set; }
}
