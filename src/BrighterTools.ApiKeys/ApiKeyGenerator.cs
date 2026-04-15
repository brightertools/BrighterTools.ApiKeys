using System.Security.Cryptography;
using System.Text;

namespace BrighterTools.ApiKeys;

internal static class ApiKeyGenerator
{
    /// <summary>
    /// Executes New Key.
    /// </summary>
    public static string NewKey(int bytes = 32)
    {
        var buffer = RandomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(buffer).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    /// <summary>
    /// Executes Hash.
    /// </summary>
    public static byte[] Hash(string input, byte[] pepper)
    {
        using var hmac = new HMACSHA256(pepper);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
    }
}

