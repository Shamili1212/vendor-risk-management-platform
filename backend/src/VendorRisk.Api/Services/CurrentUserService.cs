using System.Security.Claims;

namespace VendorRisk.Api.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string Role { get; }
}

public sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var value = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public string Role => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
