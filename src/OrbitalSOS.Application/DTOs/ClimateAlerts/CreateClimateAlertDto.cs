using System.ComponentModel.DataAnnotations;
using OrbitalSOS.Domain.Enums;
namespace OrbitalSOS.Application.DTOs.ClimateAlerts;
public class CreateClimateAlertDto
{
    public RiskType RiskType { get; set; }
    public AlertLevel Level { get; set; }
    [Required] public string AffectedRegion { get; set; } = string.Empty;
    public DateTime ExpectedAt { get; set; }
    [Required] public string Description { get; set; } = string.Empty;
}
