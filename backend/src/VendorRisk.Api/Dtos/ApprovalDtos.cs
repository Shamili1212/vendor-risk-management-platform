using VendorRisk.Api.Domain.Enums;

namespace VendorRisk.Api.Dtos;

public sealed record ApprovalRequestDto(
    Guid Id,
    Guid ContractId,
    string? ContractTitle,
    Guid RequestedById,
    Guid AssignedReviewerId,
    ApprovalStatus Status,
    DateTime? SubmittedAtUtc,
    DateTime? DecidedAtUtc,
    string? DecisionComment);

public sealed record CreateApprovalRequestDto(Guid ContractId, Guid AssignedReviewerId);
public sealed record ApprovalDecisionDto(string Comment);
public sealed record AddApprovalCommentDto(string Comment);
