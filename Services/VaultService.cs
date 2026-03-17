using System.Security.Cryptography;
using System.Text.Json;
using PasswordManager.Models;

namespace PasswordManager.Services;

public class VaultService
{
    private readonly KeyDerivationService _keyDerivationService;
    private readonly EncryptionService _encryptionService;

    public VaultService(
        KeyDerivationService keyDerivationService,
        EncryptionService encryptionService)
    {
        _keyDerivationService = keyDerivationService;
        _encryptionService = encryptionService;
    }
    
    public void CreateVault(string filePath, string masterPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(masterPassword);

        if (File.Exists(filePath))
        {
            throw new InvalidOperationException("A vault file already exists at that location.");
        }

        var emptyVault = new VaultData();
        
        SaveVault(filePath, emptyVault, masterPassword);
    }

    public VaultData OpenVault(string filePath, string masterPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(masterPassword);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("A vault file does not exist.", filePath);
        }
        
        string encryptedJson = File.ReadAllText(filePath);
        
        EncryptedVaultFile? encryptedVaultFile =
            JsonSerializer.Deserialize<EncryptedVaultFile>(encryptedJson);

        if (encryptedVaultFile is null)
        {
            throw new InvalidOperationException("Vault file could not be parsed.");
        }
        
        byte[] salt = Convert.FromBase64String(encryptedVaultFile.Salt);

        byte[] key = _keyDerivationService.DeriveKey(
            masterPassword,
            salt,
            encryptedVaultFile.Iterations);

        try
        {
            string decryptedJson = _encryptionService.Decrypt(encryptedVaultFile, key);
            
            VaultData? vault = JsonSerializer.Deserialize<VaultData>(decryptedJson);

            if (vault is null)
            {
                throw new InvalidOperationException("Vault content could not be parsed.");
            }
            
            return vault;
        }
        catch (CryptographicException)
        {
            throw new UnauthorizedAccessException("Wrong master password or corrupted vault.");
        }
        catch (FormatException)
        {
            throw new InvalidOperationException("Vault file contains invalid encoded data.");
        }
    }

    public void SaveVault(string filePath, VaultData vault, string masterPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(masterPassword);
        ArgumentNullException.ThrowIfNull(vault);
        
        byte[] salt = _keyDerivationService.GenerateSalt();

        byte[] key = _keyDerivationService.DeriveKey(
            masterPassword,
            salt,
            KeyDerivationService.DefaultIterations);
        
        string vaultJson = JsonSerializer.Serialize(vault, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        EncryptedVaultFile encryptedVaultFile = _encryptionService.Encrypt(
            plaintextJson: vaultJson,
            key: key,
            salt: salt,
            iterations: KeyDerivationService.DefaultIterations);

        string encryptedFileJson = JsonSerializer.Serialize(encryptedVaultFile, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        File.WriteAllText(filePath, encryptedFileJson);
    }
    
    
    
    
    
    
    
    
    
}