using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Domain.Entities;
public class ClimateAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public RiskType RiskType { get; set; }
    public AlertLevel Level { get; set; }
    public string AffectedRegion { get; set; } = string.Empty;
    public DateTime ExpectedAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<RiskArea> RiskAreas { get; set; } = new List<RiskArea>();
}
