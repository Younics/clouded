using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Clouded.Shared;

public static class StringExtensions
{
    public static string Encrypt(this string plainText, string key)
    {
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (
                    CryptoStream cryptoStream = new CryptoStream(
                        memoryStream,
                        encryptor,
                        CryptoStreamMode.Write
                    )
                )
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    public static string Decrypt(this string cipherText, string key)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText);

        using Aes aes = Aes.Create();

        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream memoryStream = new MemoryStream(buffer);
        using CryptoStream cryptoStream = new CryptoStream(
            memoryStream,
            decryptor,
            CryptoStreamMode.Read
        );
        using StreamReader streamReader = new StreamReader(cryptoStream);
        return streamReader.ReadToEnd();
    }

    public static string Base64Encode(this string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(this string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static string? Capitalize(this String? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        if (value.Length == 1)
            return char.ToUpper(value[0]).ToString();

        return char.ToUpper(value[0]) + value[1..];
    }

    public static string? ToCamelCase(this string? str) =>
        str is null
            ? null
            : new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }.GetResolvedPropertyName(str);

    public static string? ToSnakeCase(this string? str) =>
        str is null
            ? null
            : new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }.GetResolvedPropertyName(str);

    public static string? ToReadCase(this string? str) =>
        str is null ? null : str.Replace("_", " ").Capitalize();
}
