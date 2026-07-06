using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendorRisk.Api.Data;
using VendorRisk.Api.Dtos;

namespace VendorRisk.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public sealed class NotificationsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> List(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var items = await db.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new NotificationDto(x.Id, x.Type.ToString(), x.Title, x.Message, x.IsRead, x.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var item = await db.Notifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        if (item is null) return NotFound();

        item.IsRead = true;
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
