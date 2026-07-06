using System.Security.Cryptography;
using System.Text;

namespace VendorRisk.Api.Services;

public static class DemoPasswordHasher
{
    public static string Hash(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"vendor-risk::{password}"));
        return Convert.ToHexString(bytes);
    }

    public static bool Verify(string password, string hash) =>
        string.Equals(Hash(password), hash, StringComparison.OrdinalIgnoreCase);
}
