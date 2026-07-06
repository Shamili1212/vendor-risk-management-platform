using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendorRisk.Api.Data;
using VendorRisk.Api.Domain.Enums;
using VendorRisk.Api.Dtos;

namespace VendorRisk.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> Summary(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var in30 = today.AddDays(30);
        var in90 = today.AddDays(90);

        var riskDistribution = await db.Vendors
            .AsNoTracking()
            .GroupBy(x => x.RiskTier)
            .Select(x => new { Tier = x.Key.ToString(), Count = x.Count() })
            .ToDictionaryAsync(x => x.Tier, x => x.Count, cancellationToken);

        var recentAudit = await db.AuditLogs
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(8)
            .Select(x => new AuditLogDto(x.Id, x.ActorUserId, x.Action, x.EntityName, x.EntityId, x.Details, x.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(new DashboardSummaryDto(
            await db.Vendors.CountAsync(cancellationToken),
            await db.Vendors.CountAsync(x => x.RiskTier == RiskTier.High || x.RiskTier == RiskTier.Critical, cancellationToken),
            await db.Contracts.CountAsync(x => x.RenewalDate >= today && x.RenewalDate <= in30, cancellationToken),
            await db.Contracts.CountAsync(x => x.RenewalDate >= today && x.RenewalDate <= in90, cancellationToken),
            await db.ApprovalRequests.CountAsync(x => x.Status == ApprovalStatus.Submitted, cancellationToken),
            riskDistribution,
            recentAudit));
    }
}
