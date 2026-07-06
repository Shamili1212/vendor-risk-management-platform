using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendorRisk.Api.Data;
using VendorRisk.Api.Dtos;
using VendorRisk.Api.Services;

namespace VendorRisk.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AppDbContext db, ITokenService tokens, IAuditService audit) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email && x.IsActive, cancellationToken);
        if (user is null || !DemoPasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized();
        }

        var token = tokens.CreateToken(user);
        await audit.WriteAsync("Login", "User", user.Id, "User signed in.", cancellationToken);

        return Ok(new AuthResponse(token.Token, token.ExpiresAtUtc, new UserProfileDto(user.Id, user.FullName, user.Email, user.Role)));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> Me(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var user = await db.Users.FindAsync([userId], cancellationToken);
        return user is null
            ? NotFound()
            : Ok(new UserProfileDto(user.Id, user.FullName, user.Email, user.Role));
    }
}
