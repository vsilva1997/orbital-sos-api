using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Domain.Entities;
public class RiskArea
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClimateAlertId { get; set; }
    public ClimateAlert ClimateAlert { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string PolygonGeoJson { get; set; } = string.Empty;
    public DangerLevel DangerLevel { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
