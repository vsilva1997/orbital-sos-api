using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Application.DTOs.ClimateAlerts;
public class ClimateAlertDto
{
    public Guid Id { get; set; }
    public RiskType RiskType { get; set; }
    public AlertLevel Level { get; set; }
    public string AffectedRegion { get; set; } = string.Empty;
    public DateTime ExpectedAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
