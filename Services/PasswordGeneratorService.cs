using System.Security.Cryptography;

namespace PasswordManager.Services;

public class PasswordGeneratorService
{
    private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string SpecialCharacters = "!@#$%^&*()-_=+[]{};:,.?";

    public string GeneratePassword(int length = 20)
    {
        if (length < 12)
        {
            throw new ArgumentException("Password length must be at least 12 characters.");
        }

        string allCharacters = Lowercase + Uppercase + Digits + SpecialCharacters;

        var passwordChars = new List<char>
        {
            GetRandomChar(Lowercase),
            GetRandomChar(Uppercase),
            GetRandomChar(Digits),
            GetRandomChar(SpecialCharacters)
        };

        while (passwordChars.Count < length)
        {
            passwordChars.Add(GetRandomChar(allCharacters));
        }

        Shuffle(passwordChars);

        return new string(passwordChars.ToArray());
    }

    private static char GetRandomChar(string characters)
    {
        int index = RandomNumberGenerator.GetInt32(characters.Length);
        return characters[index];
    }

    private static void Shuffle(List<char> characters)
    {
        for (int i = characters.Count - 1; i > 0; i--)
        {
            int swapIndex = RandomNumberGenerator.GetInt32(i + 1);
            (characters[i], characters[swapIndex]) = (characters[swapIndex], characters[i]);
        }
    }
}