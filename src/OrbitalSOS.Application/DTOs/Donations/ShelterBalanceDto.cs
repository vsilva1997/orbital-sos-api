namespace OrbitalSOS.Application.DTOs.Donations;
public class ShelterBalanceDto
{
    public Guid ShelterId { get; set; }
    public string ShelterName { get; set; } = string.Empty;
    public IEnumerable<ItemBalanceDto> Items { get; set; } = Enumerable.Empty<ItemBalanceDto>();
}
public class ItemBalanceDto
{
    public string ItemType { get; set; } = string.Empty;
    public int QuantityNeeded { get; set; }
    public int QuantityDonated { get; set; }
    public int Gap => Math.Max(0, QuantityNeeded - QuantityDonated);
    public bool IsSatisfied => QuantityDonated >= QuantityNeeded;
}
