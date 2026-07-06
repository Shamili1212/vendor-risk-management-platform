using VendorRisk.Api.Domain.Enums;

namespace VendorRisk.Api.Domain.Entities;

public sealed class Vendor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public VendorStatus Status { get; set; } = VendorStatus.UnderReview;
    public Criticality Criticality { get; set; } = Criticality.Medium;
    public ComplianceStatus ComplianceStatus { get; set; } = ComplianceStatus.ReviewRequired;
    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }
    public RiskTier RiskTier { get; set; } = RiskTier.Medium;
    public int IncidentCount { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
