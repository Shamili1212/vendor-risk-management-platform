using VendorRisk.Api.Domain.Enums;

namespace VendorRisk.Api.Dtos;

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResponse(
    string Token,
    DateTime ExpiresAtUtc,
    UserProfileDto User);

public sealed record UserProfileDto(
    Guid Id,
    string FullName,
    string Email,
    UserRole Role);
