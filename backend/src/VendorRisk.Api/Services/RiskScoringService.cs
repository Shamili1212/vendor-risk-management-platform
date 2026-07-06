using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using VendorRisk.Api.Domain.Entities;
using VendorRisk.Api.Domain.Enums;
using VendorRisk.Api.Dtos;

namespace VendorRisk.Api.Services;

public interface IRiskScoringService
{
    Task<RiskEngineResult> CalculateAsync(Vendor vendor, Contract? contract, CancellationToken cancellationToken);
}

public sealed class RiskScoringService(IConfiguration configuration, ILogger<RiskScoringService> logger) : IRiskScoringService
{
    public async Task<RiskEngineResult> CalculateAsync(Vendor vendor, Contract? contract, CancellationToken cancellationToken)
    {
        var enginePath = configuration["RiskEngine:Path"];
        if (!string.IsNullOrWhiteSpace(enginePath) && File.Exists(enginePath))
        {
            try
            {
                return await CalculateWithCliAsync(enginePath, vendor, contract, cancellationToken);
            }
            catch (Exception ex) when (ex is IOException or InvalidOperationException or JsonException)
            {
                logger.LogWarning(ex, "Risk engine CLI failed; using managed fallback scoring.");
            }
        }

        return CalculateManaged(vendor, contract);
    }

    private static async Task<RiskEngineResult> CalculateWithCliAsync(string enginePath, Vendor vendor, Contract? contract, CancellationToken cancellationToken)
    {
        var renewalDays = contract is null ? 365 : Math.Max(0, contract.RenewalDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber);
        var args = string.Join(' ', new[]
        {
            ((int)vendor.Criticality).ToString(CultureInfo.InvariantCulture),
            ((int)vendor.ComplianceStatus).ToString(CultureInfo.InvariantCulture),
            vendor.IncidentCount.ToString(CultureInfo.InvariantCulture),
            (contract?.Value ?? 0m).ToString(CultureInfo.InvariantCulture),
            renewalDays.ToString(CultureInfo.InvariantCulture)
        });

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = enginePath,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });

        if (process is null)
        {
            throw new InvalidOperationException("Could not start risk engine process.");
        }

        var output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        var error = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Risk engine exited with {process.ExitCode}: {error}");
        }

        var result = JsonSerializer.Deserialize<RiskEngineCliResult>(output, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Risk engine returned an empty payload.");

        return new RiskEngineResult(result.Score, ParseTier(result.Tier), result.Rationale);
    }

    private static RiskEngineResult CalculateManaged(Vendor vendor, Contract? contract)
    {
        var score = 0;
        score += vendor.Criticality switch { Criticality.Critical => 30, Criticality.High => 22, Criticality.Medium => 12, _ => 5 };
        score += vendor.ComplianceStatus switch { ComplianceStatus.NonCompliant => 30, ComplianceStatus.ReviewRequired => 16, _ => 2 };
        score += Math.Min(20, vendor.IncidentCount * 5);
        score += contract is null
            ? 0
            : contract.Value switch { >= 1_000_000m => 15, >= 250_000m => 10, >= 50_000m => 5, _ => 1 };

        if (contract is not null)
        {
            var days = contract.RenewalDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
            score += days switch { <= 30 => 15, <= 90 => 8, <= 180 => 4, _ => 0 };
        }

        score = Math.Clamp(score, 0, 100);
        var tier = score switch { >= 80 => RiskTier.Critical, >= 60 => RiskTier.High, >= 35 => RiskTier.Medium, _ => RiskTier.Low };
        return new RiskEngineResult(score, tier, $"Score {score}: {vendor.Criticality} criticality, {vendor.ComplianceStatus} compliance, {vendor.IncidentCount} incidents, contract value {contract?.Value ?? 0m:C}.");
    }

    private static RiskTier ParseTier(string value) =>
        Enum.TryParse<RiskTier>(value, true, out var tier) ? tier : RiskTier.Medium;

    private sealed record RiskEngineCliResult(int Score, string Tier, string Rationale);
}
