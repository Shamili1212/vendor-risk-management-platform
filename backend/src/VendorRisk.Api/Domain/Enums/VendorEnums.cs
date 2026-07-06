namespace VendorRisk.Api.Domain.Enums;

public enum VendorStatus
{
    Active,
    UnderReview,
    Suspended,
    Offboarded
}

public enum Criticality
{
    Low,
    Medium,
    High,
    Critical
}

public enum ComplianceStatus
{
    Compliant,
    ReviewRequired,
    NonCompliant
}

public enum RiskTier
{
    Low,
    Medium,
    High,
    Critical
}
