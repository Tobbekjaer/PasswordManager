using System.Security.Cryptography;
using System.Text;
using PasswordManager.Models;

namespace PasswordManager.Services;

public class EncryptionService
{
    public const int NonceSize = 12; // Recommended size for AES-GCM
    public const int TagSize = 16;   // 128-bit authentication tag

    public EncryptedVaultFile Encrypt(string plaintextJson, byte[] key, byte[] salt, int iterations)
    {
        ArgumentNullException.ThrowIfNull(plaintextJson);
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(salt);
        
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintextJson);
        byte[] ciphertext = new byte[plaintextBytes.Length];
        byte[] tag = new byte[TagSize];

        using var aesGcm = new AesGcm(key, TagSize);
        aesGcm.Encrypt(
            nonce: nonce,
            plaintext: plaintextBytes,
            ciphertext: ciphertext,
            tag: tag);

        return new EncryptedVaultFile
        {
            Salt = Convert.ToBase64String(salt),
            Nonce = Convert.ToBase64String(nonce),
            Ciphertext = Convert.ToBase64String(ciphertext),
            Tag = Convert.ToBase64String(tag),
            Iterations = iterations
        };
    }

    public string Decrypt(EncryptedVaultFile encryptedVaultFile, byte[] key)
    {
        ArgumentNullException.ThrowIfNull(encryptedVaultFile);
        ArgumentNullException.ThrowIfNull(key);
        
        byte[] nonce = Convert.FromBase64String(encryptedVaultFile.Nonce);
        byte[] ciphertext = Convert.FromBase64String(encryptedVaultFile.Ciphertext);
        byte[] tag = Convert.FromBase64String(encryptedVaultFile.Tag);

        byte[] plaintextBytes = new byte[ciphertext.Length];
        
        using var aesGcm = new AesGcm(key, TagSize);
        aesGcm.Decrypt(
            nonce: nonce,
            ciphertext: ciphertext,
            tag: tag,
            plaintext: plaintextBytes);
        
        return Encoding.UTF8.GetString(plaintextBytes);
    }
    
    
}





