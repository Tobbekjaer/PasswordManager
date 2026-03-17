using System.Security.Cryptography;

namespace PasswordManager.Services;

public class KeyDerivationService
{
    public const int SaltSize = 16;
    public const int KeySize = 32; // 256-bit key for AES-256
    public const int DefaultIterations = 200_000;

    public byte[] GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(SaltSize);
    }

    public byte[] DeriveKey(string masterPassword, byte[] salt, int iterations = DefaultIterations)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(masterPassword);

        return Rfc2898DeriveBytes.Pbkdf2(
            password: masterPassword,
            salt: salt,
            iterations: iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: KeySize);
    }
    
    
    
    
    
}