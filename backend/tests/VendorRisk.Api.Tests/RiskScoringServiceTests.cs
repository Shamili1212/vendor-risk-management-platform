using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using VendorRisk.Api.Domain.Entities;
using VendorRisk.Api.Domain.Enums;
using VendorRisk.Api.Services;
using Xunit;

namespace VendorRisk.Api.Tests;

public sealed class RiskScoringServiceTests
{
    [Fact]
    public async Task CalculateAsync_UsesManagedFallback_WhenCliPathIsMissing()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["RiskEngine:Path"] = "missing-risk-engine" })
            .Build();
        var service = new RiskScoringService(config, NullLogger<RiskScoringService>.Instance);

        var vendor = new Vendor
        {
            Criticality = Criticality.Critical,
            ComplianceStatus = ComplianceStatus.NonCompliant,
            IncidentCount = 4
        };
        var contract = new Contract
        {
            Value = 1_200_000m,
            RenewalDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10)
        };

        var result = await service.CalculateAsync(vendor, contract, CancellationToken.None);

        Assert.Equal(RiskTier.Critical, result.Tier);
        Assert.InRange(result.Score, 80, 100);
        Assert.Contains("Score", result.Rationale);
    }
}
