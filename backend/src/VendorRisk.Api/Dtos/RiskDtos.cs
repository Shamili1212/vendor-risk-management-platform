using VendorRisk.Api.Domain.Enums;

namespace VendorRisk.Api.Dtos;

public sealed record RiskAssessmentDto(
    Guid Id,
    Guid VendorId,
    Guid? ContractId,
    int Score,
    RiskTier Tier,
    string Rationale,
    DateTime CalculatedAtUtc);

public sealed record RiskEngineResult(int Score, RiskTier Tier, string Rationale);
