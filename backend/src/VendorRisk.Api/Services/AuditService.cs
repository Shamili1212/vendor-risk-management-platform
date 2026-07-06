using VendorRisk.Api.Data;
using VendorRisk.Api.Domain.Entities;

namespace VendorRisk.Api.Services;

public interface IAuditService
{
    Task WriteAsync(string action, string entityName, Guid? entityId, string details, CancellationToken cancellationToken);
}

public sealed class AuditService(AppDbContext db, ICurrentUserService currentUser, IHttpContextAccessor accessor) : IAuditService
{
    public async Task WriteAsync(string action, string entityName, Guid? entityId, string details, CancellationToken cancellationToken)
    {
        db.AuditLogs.Add(new AuditLog
        {
            ActorUserId = currentUser.UserId == Guid.Empty ? null : currentUser.UserId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Details = details,
            IpAddress = accessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            CreatedAtUtc = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}
