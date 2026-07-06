using Microsoft.EntityFrameworkCore;
using VendorRisk.Api.Data;
using VendorRisk.Api.Domain.Entities;
using VendorRisk.Api.Domain.Enums;

namespace VendorRisk.Api.Services;

public interface IApprovalWorkflowService
{
    Task<ApprovalRequest> SubmitAsync(Guid id, CancellationToken cancellationToken);
    Task<ApprovalRequest> DecideAsync(Guid id, ApprovalStatus targetStatus, string comment, CancellationToken cancellationToken);
}

public sealed class ApprovalWorkflowService(AppDbContext db, IAuditService audit) : IApprovalWorkflowService
{
    public async Task<ApprovalRequest> SubmitAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = await db.ApprovalRequests.FindAsync([id], cancellationToken)
            ?? throw new KeyNotFoundException("Approval request was not found.");

        if (request.Status is not (ApprovalStatus.Draft or ApprovalStatus.ChangesRequested))
        {
            throw new InvalidOperationException("Only draft or changes-requested approvals can be submitted.");
        }

        request.Status = ApprovalStatus.Submitted;
        request.SubmittedAtUtc = DateTime.UtcNow;
        request.UpdatedAtUtc = DateTime.UtcNow;

        db.Notifications.Add(new Notification
        {
            UserId = request.AssignedReviewerId,
            Type = NotificationType.ApprovalAssigned,
            Title = "Approval assigned",
            Message = "A contract approval request is waiting for your review."
        });

        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("Submit", nameof(ApprovalRequest), request.Id, "Approval request submitted.", cancellationToken);
        return request;
    }

    public async Task<ApprovalRequest> DecideAsync(Guid id, ApprovalStatus targetStatus, string comment, CancellationToken cancellationToken)
    {
        if (targetStatus is not (ApprovalStatus.Approved or ApprovalStatus.Rejected or ApprovalStatus.ChangesRequested))
        {
            throw new ArgumentOutOfRangeException(nameof(targetStatus), "Unsupported approval decision.");
        }

        var request = await db.ApprovalRequests.Include(x => x.Contract).FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Approval request was not found.");

        if (request.Status != ApprovalStatus.Submitted)
        {
            throw new InvalidOperationException("Only submitted approvals can be decided.");
        }

        request.Status = targetStatus;
        request.DecisionComment = comment;
        request.DecidedAtUtc = DateTime.UtcNow;
        request.UpdatedAtUtc = DateTime.UtcNow;

        db.Notifications.Add(new Notification
        {
            UserId = request.RequestedById,
            Type = NotificationType.ApprovalDecision,
            Title = $"Approval {targetStatus}",
            Message = $"Your approval request for {request.Contract?.Title ?? "a contract"} was {targetStatus}."
        });

        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("ApprovalDecision", nameof(ApprovalRequest), request.Id, $"Decision: {targetStatus}. {comment}", cancellationToken);
        return request;
    }
}
