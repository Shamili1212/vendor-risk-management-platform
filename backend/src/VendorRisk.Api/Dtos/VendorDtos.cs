using VendorRisk.Api.Domain.Enums;

namespace VendorRisk.Api.Dtos;

public sealed record VendorDto(
    Guid Id,
    string Name,
    string Category,
    VendorStatus Status,
    Criticality Criticality,
    ComplianceStatus ComplianceStatus,
    Guid OwnerId,
    string? OwnerName,
    RiskTier RiskTier,
    int IncidentCount);

public sealed record CreateVendorRequest(
    string Name,
    string Category,
    VendorStatus Status,
    Criticality Criticality,
    ComplianceStatus ComplianceStatus,
    Guid OwnerId,
    int IncidentCount);

public sealed record UpdateVendorRequest(
    string Name,
    string Category,
    VendorStatus Status,
    Criticality Criticality,
    ComplianceStatus ComplianceStatus,
    Guid OwnerId,
    int IncidentCount);
