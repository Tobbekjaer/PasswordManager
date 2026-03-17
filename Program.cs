using PasswordManager.Models;
using PasswordManager.Services;

var keyDerivationService = new KeyDerivationService();
var encryptionService = new EncryptionService();
var vaultService = new VaultService(keyDerivationService, encryptionService);

string filePath = "vault.json";
string masterPassword = "MyStrongMasterPassword123!";

Console.WriteLine("=== STEP 2 TEST ===");
Console.WriteLine();

if (!File.Exists(filePath))
{
    Console.WriteLine("Creating new vault...");
    vaultService.CreateVault(filePath, masterPassword);
    Console.WriteLine("Vault created.");
    Console.WriteLine();
}

try
{
    Console.WriteLine("Opening vault...");
    VaultData vault = vaultService.OpenVault(filePath, masterPassword);
    Console.WriteLine("Vault opened successfully.");
    Console.WriteLine();

    Console.WriteLine($"Credentials before adding: {vault.Credentials.Count}");

    vault.Credentials.Add(new Credential
    {
        Site = "facebook",
        Username = "tobias@example.com",
        Password = "SuperSecretPassword!"
    });

    vault.Credentials.Add(new Credential
    {
        Site = "github",
        Username = "tobiasdev",
        Password = "AnotherStrongPassword!"
    });

    Console.WriteLine($"Credentials after adding: {vault.Credentials.Count}");
    Console.WriteLine("Saving vault...");
    vaultService.SaveVault(filePath, vault, masterPassword);
    Console.WriteLine("Vault saved.");
    Console.WriteLine();

    Console.WriteLine("Re-opening vault to verify...");
    VaultData reopenedVault = vaultService.OpenVault(filePath, masterPassword);

    Console.WriteLine($"Reopened vault contains {reopenedVault.Credentials.Count} credentials:");
    Console.WriteLine();

    foreach (var credential in reopenedVault.Credentials)
    {
        Console.WriteLine($"Site: {credential.Site}");
        Console.WriteLine($"Username: {credential.Username}");
        Console.WriteLine($"Password: {credential.Password}");
        Console.WriteLine();
    }
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Access denied: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}