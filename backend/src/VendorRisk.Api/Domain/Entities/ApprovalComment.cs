namespace VendorRisk.Api.Domain.Entities;

public sealed class ApprovalComment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ApprovalRequestId { get; set; }
    public ApprovalRequest? ApprovalRequest { get; set; }
    public Guid AuthorId { get; set; }
    public User? Author { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
