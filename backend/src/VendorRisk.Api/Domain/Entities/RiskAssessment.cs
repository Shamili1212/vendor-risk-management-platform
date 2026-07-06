using VendorRisk.Api.Domain.Enums;

namespace VendorRisk.Api.Domain.Entities;

public sealed class RiskAssessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VendorId { get; set; }
    public Vendor? Vendor { get; set; }
    public Guid? ContractId { get; set; }
    public Contract? Contract { get; set; }
    public int Score { get; set; }
    public RiskTier Tier { get; set; }
    public string Rationale { get; set; } = string.Empty;
    public Guid CalculatedById { get; set; }
    public User? CalculatedBy { get; set; }
    public DateTime CalculatedAtUtc { get; set; } = DateTime.UtcNow;
}
