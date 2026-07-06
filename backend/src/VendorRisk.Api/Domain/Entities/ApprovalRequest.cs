using VendorRisk.Api.Domain.Enums;

namespace VendorRisk.Api.Domain.Entities;

public sealed class ApprovalRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ContractId { get; set; }
    public Contract? Contract { get; set; }
    public Guid RequestedById { get; set; }
    public User? RequestedBy { get; set; }
    public Guid AssignedReviewerId { get; set; }
    public User? AssignedReviewer { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Draft;
    public DateTime? SubmittedAtUtc { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
    public string? DecisionComment { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public ICollection<ApprovalComment> Comments { get; set; } = new List<ApprovalComment>();
}
