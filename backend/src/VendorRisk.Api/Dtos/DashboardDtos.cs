namespace VendorRisk.Api.Dtos;

public sealed record DashboardSummaryDto(
    int TotalVendors,
    int HighRiskVendors,
    int ExpiringContracts30Days,
    int ExpiringContracts90Days,
    int PendingApprovals,
    IReadOnlyDictionary<string, int> RiskDistribution,
    IReadOnlyList<AuditLogDto> RecentAuditEvents);

public sealed record AuditLogDto(
    Guid Id,
    Guid? ActorUserId,
    string Action,
    string EntityName,
    Guid? EntityId,
    string Details,
    DateTime CreatedAtUtc);

public sealed record NotificationDto(
    Guid Id,
    string Type,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAtUtc);
