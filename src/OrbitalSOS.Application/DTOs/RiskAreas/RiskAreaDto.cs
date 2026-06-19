using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Application.DTOs.RiskAreas;
public class RiskAreaDto
{
    public Guid Id { get; set; }
    public Guid ClimateAlertId { get; set; }
    public string ClimateAlertRegion { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PolygonGeoJson { get; set; } = string.Empty;
    public DangerLevel DangerLevel { get; set; }
    public DateTime CreatedAt { get; set; }
}
