namespace Clouded.Shared;

public static class Generator
{
    public static string RandomString(
        int length = 15,
        bool includeNumbers = true,
        bool includeSpecialCharacters = true
    )
    {
        var validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        if (includeNumbers)
            validChars += "0123456789";

        if (includeSpecialCharacters)
            validChars += "!@#$%^&*?_-";

        var random = new Random();

        var chars = new char[length];
        for (var i = 0; i < length; i++)
            chars[i] = validChars[random.Next(0, validChars.Length)];

        return new string(chars);
    }
}
