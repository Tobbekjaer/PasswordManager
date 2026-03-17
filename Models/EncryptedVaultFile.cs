namespace PasswordManager.Models;

public class EncryptedVaultFile
{
    public string Salt { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
    public string Ciphertext { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public int Iterations { get; set; }
}