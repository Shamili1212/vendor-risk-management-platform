using VendorRisk.Api.Domain.Enums;

namespace VendorRisk.Api.Domain.Entities;

public sealed class Contract
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VendorId { get; set; }
    public Vendor? Vendor { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly RenewalDate { get; set; }
    public decimal Value { get; set; }
    public string Currency { get; set; } = "USD";
    public ContractStatus Status { get; set; } = ContractStatus.Draft;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public ICollection<DocumentMetadata> Documents { get; set; } = new List<DocumentMetadata>();
    public ICollection<ApprovalRequest> ApprovalRequests { get; set; } = new List<ApprovalRequest>();
}
