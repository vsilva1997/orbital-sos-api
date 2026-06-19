using System.ComponentModel.DataAnnotations;
using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Application.DTOs.Shelters;
public class CreateShelterDto
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string Address { get; set; } = string.Empty;
    [Range(-90, 90)] public double Latitude { get; set; }
    [Range(-180, 180)] public double Longitude { get; set; }
    [Range(1, 100000)] public int Capacity { get; set; }
    public ShelterType Type { get; set; }
}
