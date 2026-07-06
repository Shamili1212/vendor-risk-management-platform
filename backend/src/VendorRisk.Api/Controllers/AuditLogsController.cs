using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendorRisk.Api.Data;
using VendorRisk.Api.Dtos;

namespace VendorRisk.Api.Controllers;

[ApiController]
[Authorize(Policy = "AuditorRead")]
[Route("api/audit-logs")]
public sealed class AuditLogsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> Search(
        [FromQuery] Guid? actorUserId,
        [FromQuery] string? action,
        [FromQuery] string? entityName,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var query = db.AuditLogs.AsNoTracking();
        if (actorUserId is not null) query = query.Where(x => x.ActorUserId == actorUserId);
        if (!string.IsNullOrWhiteSpace(action)) query = query.Where(x => x.Action == action);
        if (!string.IsNullOrWhiteSpace(entityName)) query = query.Where(x => x.EntityName == entityName);
        if (from is not null) query = query.Where(x => x.CreatedAtUtc >= from);
        if (to is not null) query = query.Where(x => x.CreatedAtUtc <= to);

        return Ok(await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(200)
            .Select(x => new AuditLogDto(x.Id, x.ActorUserId, x.Action, x.EntityName, x.EntityId, x.Details, x.CreatedAtUtc))
            .ToListAsync(cancellationToken));
    }
}
