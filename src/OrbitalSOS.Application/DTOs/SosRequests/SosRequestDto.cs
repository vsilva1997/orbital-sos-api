using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Application.DTOs.SosRequests;
public class SosRequestDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid? SuggestedShelterId { get; set; }
    public string? SuggestedShelterName { get; set; }
    public EmergencyType EmergencyType { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public SosStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
