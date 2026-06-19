using System.ComponentModel.DataAnnotations;
using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Application.DTOs.SosRequests;
public class CreateSosRequestDto
{
    public EmergencyType EmergencyType { get; set; }
    [Range(-90, 90)] public double Latitude { get; set; }
    [Range(-180, 180)] public double Longitude { get; set; }
    public Guid? SuggestedShelterId { get; set; }
    public string? Notes { get; set; }
}
