using System.Security.Cryptography;
using System.Text;

namespace WebBanGiay.Helpers;

public static class PasswordHelper
{
    public static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

    public static bool Verify(string input, string stored)
    {
        if (string.IsNullOrWhiteSpace(stored))
        {
            return false;
        }

        var hashed = Hash(input);
        return string.Equals(stored, hashed, StringComparison.OrdinalIgnoreCase)
            || string.Equals(stored, input, StringComparison.Ordinal);
    }
}
