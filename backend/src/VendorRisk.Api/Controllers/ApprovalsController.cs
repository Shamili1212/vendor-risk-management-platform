using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendorRisk.Api.Data;
using VendorRisk.Api.Domain.Entities;
using VendorRisk.Api.Domain.Enums;
using VendorRisk.Api.Dtos;
using VendorRisk.Api.Services;

namespace VendorRisk.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/approvals")]
public sealed class ApprovalsController(AppDbContext db, IApprovalWorkflowService workflow, IAuditService audit) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ApprovalRequestDto>>> List(CancellationToken cancellationToken)
    {
        var query = db.ApprovalRequests.Include(x => x.Contract).AsNoTracking();
        if (User.IsInRole(UserRole.Reviewer.ToString()))
        {
            var userId = User.GetUserId();
            query = query.Where(x => x.AssignedReviewerId == userId);
        }

        return Ok(await query.OrderByDescending(x => x.CreatedAtUtc).Select(x => ToDto(x)).ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApprovalRequestDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var request = await db.ApprovalRequests.Include(x => x.Contract).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return request is null ? NotFound() : Ok(ToDto(request));
    }

    [HttpPost]
    [Authorize(Policy = "ProcurementOrAdmin")]
    public async Task<ActionResult<ApprovalRequestDto>> Create(CreateApprovalRequestDto request, CancellationToken cancellationToken)
    {
        if (!await db.Contracts.AnyAsync(x => x.Id == request.ContractId, cancellationToken)) return BadRequest("Contract does not exist.");
        if (!await db.Users.AnyAsync(x => x.Id == request.AssignedReviewerId && x.Role == UserRole.Reviewer, cancellationToken)) return BadRequest("Assigned reviewer must be a Reviewer user.");

        var approval = new ApprovalRequest
        {
            ContractId = request.ContractId,
            RequestedById = User.GetUserId(),
            AssignedReviewerId = request.AssignedReviewerId
        };

        db.ApprovalRequests.Add(approval);
        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("Create", nameof(ApprovalRequest), approval.Id, "Approval request created.", cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = approval.Id }, ToDto(approval));
    }

    [HttpPost("{id:guid}/submit")]
    [Authorize(Policy = "ProcurementOrAdmin")]
    public async Task<ActionResult<ApprovalRequestDto>> Submit(Guid id, CancellationToken cancellationToken)
    {
        var request = await workflow.SubmitAsync(id, cancellationToken);
        return Ok(ToDto(request));
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = "ReviewerOrAdmin")]
    public async Task<ActionResult<ApprovalRequestDto>> Approve(Guid id, ApprovalDecisionDto decision, CancellationToken cancellationToken)
    {
        var request = await workflow.DecideAsync(id, ApprovalStatus.Approved, decision.Comment, cancellationToken);
        return Ok(ToDto(request));
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = "ReviewerOrAdmin")]
    public async Task<ActionResult<ApprovalRequestDto>> Reject(Guid id, ApprovalDecisionDto decision, CancellationToken cancellationToken)
    {
        var request = await workflow.DecideAsync(id, ApprovalStatus.Rejected, decision.Comment, cancellationToken);
        return Ok(ToDto(request));
    }

    [HttpPost("{id:guid}/request-changes")]
    [Authorize(Policy = "ReviewerOrAdmin")]
    public async Task<ActionResult<ApprovalRequestDto>> RequestChanges(Guid id, ApprovalDecisionDto decision, CancellationToken cancellationToken)
    {
        var request = await workflow.DecideAsync(id, ApprovalStatus.ChangesRequested, decision.Comment, cancellationToken);
        return Ok(ToDto(request));
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, AddApprovalCommentDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Comment)) return BadRequest("Comment is required.");
        if (!await db.ApprovalRequests.AnyAsync(x => x.Id == id, cancellationToken)) return NotFound();

        db.ApprovalComments.Add(new ApprovalComment
        {
            ApprovalRequestId = id,
            AuthorId = User.GetUserId(),
            Comment = request.Comment.Trim()
        });

        await db.SaveChangesAsync(cancellationToken);
        await audit.WriteAsync("Create", nameof(ApprovalComment), id, "Approval comment added.", cancellationToken);
        return NoContent();
    }

    private static ApprovalRequestDto ToDto(ApprovalRequest request) =>
        new(request.Id, request.ContractId, request.Contract?.Title, request.RequestedById, request.AssignedReviewerId, request.Status, request.SubmittedAtUtc, request.DecidedAtUtc, request.DecisionComment);
}
