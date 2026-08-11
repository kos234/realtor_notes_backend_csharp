using System.Security.Cryptography;

namespace realtor_notes_backend.utils;

public static class Utils
{
    public static string Random16Bytes()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToBase64String(bytes);
    }
}