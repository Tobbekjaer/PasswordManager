using System.Text.Json;
using PasswordManager.Models;
using PasswordManager.Services;

var keyDerivationService = new KeyDerivationService();
var encryptionService = new EncryptionService();

const string masterPassword = "MyStrongMasterPassword123!";

var vault = new VaultData
{
    Credentials =
    {
        new Credential
        {
            Site = "facebook",
            Username = "tobias@example.com",
            Password = "SuperSecretPassword!"
        },
        new Credential
        {
            Site = "github",
            Username = "tobiasdev",
            Password = "AnotherVeryStrongPassword!"
        }
    }
};

string vaultJson = JsonSerializer.Serialize(vault, new JsonSerializerOptions
{
    WriteIndented = true
});

Console.WriteLine("Original vault JSON:");
Console.WriteLine(vaultJson);
Console.WriteLine();

byte[] salt = keyDerivationService.GenerateSalt();
byte[] key = keyDerivationService.DeriveKey(
    masterPassword,
    salt,
    KeyDerivationService.DefaultIterations);

EncryptedVaultFile encryptedVault = encryptionService.Encrypt(
    plaintextJson: vaultJson,
    key: key,
    salt: salt,
    iterations: KeyDerivationService.DefaultIterations);

Console.WriteLine("Encrypted vault file:");
Console.WriteLine(JsonSerializer.Serialize(encryptedVault, new JsonSerializerOptions
{
    WriteIndented = true
}));
Console.WriteLine();

byte[] derivedKeyAgain = keyDerivationService.DeriveKey(
    masterPassword,
    Convert.FromBase64String(encryptedVault.Salt),
    encryptedVault.Iterations);

string decryptedJson = encryptionService.Decrypt(encryptedVault, derivedKeyAgain);

Console.WriteLine("Decrypted vault JSON:");
Console.WriteLine(decryptedJson);
Console.WriteLine();

VaultData? restoredVault = JsonSerializer.Deserialize<VaultData>(decryptedJson);

Console.WriteLine("Restored credentials:");
if (restoredVault is not null)
{
    foreach (var credential in restoredVault.Credentials)
    {
        Console.WriteLine($"Site: {credential.Site}");
        Console.WriteLine($"Username: {credential.Username}");
        Console.WriteLine($"Password: {credential.Password}");
        Console.WriteLine();
    }
}