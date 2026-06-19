using System.ComponentModel.DataAnnotations;
using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Application.DTOs.RiskAreas;
public class CreateRiskAreaDto
{
    [Required] public Guid ClimateAlertId { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string PolygonGeoJson { get; set; } = string.Empty;
    public DangerLevel DangerLevel { get; set; }
}
