using PasswordManager.Models;
using PasswordManager.Utilities;

namespace PasswordManager.Services;

public class ConsoleUiService
{
    private readonly VaultService _vaultService;
    private readonly PasswordGeneratorService _passwordGeneratorService;

    public ConsoleUiService(VaultService vaultService, 
        PasswordGeneratorService passwordGeneratorService)
    {
        _vaultService = vaultService;
        _passwordGeneratorService = passwordGeneratorService;
    }

    public void Run()
    {
        const string filePath = "vault.json";

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to Password Manager");
            Console.WriteLine("1: Create vault");
            Console.WriteLine("2: Open vault");
            Console.WriteLine("3: Exit");
            Console.Write("Choose an option: ");
            
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateVaultFlow(filePath);
                    break;
                case "2":
                    OpenVaultFlow(filePath);
                    break;
                case "3":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    ShowMessage("Invalid option. Try again.");
                    break;
            }
        }
    }

    private void CreateVaultFlow(string filePath)
    {
        Console.Clear();
        Console.WriteLine("Create new vault");
        
        string masterPassword = SecureConsole.ReadHiddenInput("Enter master password: ");

        if (string.IsNullOrWhiteSpace(masterPassword))
        {
            ShowMessage("Master password cannot be empty.");
            return;
        }

        try
        {
            _vaultService.CreateVault(filePath, masterPassword);
            ShowMessage("Vault created successfully.");
        }
        catch (Exception ex)
        {
            ShowMessage($"Error: {ex.Message}");
        }        
    }

    private void OpenVaultFlow(string filePath)
    {
        Console.Clear();
        Console.WriteLine("Open vault");
        
        string masterPassword = SecureConsole.ReadHiddenInput("Enter master password: ");

        if (string.IsNullOrWhiteSpace(masterPassword))
        {
            ShowMessage("Master password cannot be empty.");
            return;
        }

        try
        {
            VaultData vault = _vaultService.OpenVault(filePath, masterPassword);
            VaultMenu(filePath, vault, masterPassword);
        }
        catch (Exception ex)
        {
            ShowMessage($"Error: {ex.Message}");
        }
    }

    private void VaultMenu(string filePath, VaultData vault, string masterPassword)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Vault is unlocked. What do you want to do?");
            Console.WriteLine("1: Add credential");
            Console.WriteLine("2: List all credentials");
            Console.WriteLine("3: Show credential");
            Console.WriteLine("4: Remove credential");
            Console.WriteLine("5: Generate password");
            Console.WriteLine("6: Save and exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddCredentialFlow(vault);
                    break;
                case "2":
                    ListCredentialsFlow(vault);
                    break;
                case "3":
                    ShowCredentialFlow(vault);
                    break;
                case "4":
                    RemoveCredentialFlow(vault);
                    break;
                case "5":
                    GeneratePasswordFlow();
                    break;
                case "6":
                    try
                    {
                        _vaultService.SaveVault(filePath, vault, masterPassword);
                        ShowMessage("Vault saved.");
                        return;
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Error while saving vault: {ex.Message}");
                    }
                    break;
                default:
                    ShowMessage("Invalid option. Try again.");
                    break;
            }
        }
    }

    private void AddCredentialFlow(VaultData vault)
    {
        Console.Clear();
        Console.WriteLine("Add credential");

        Console.Write("Site: ");
        string? site = Console.ReadLine();

        Console.Write("Username: ");
        string? username = Console.ReadLine();

        string password = SecureConsole.ReadHiddenInput("Password: ");

        if (string.IsNullOrWhiteSpace(site) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            ShowMessage("All fields are required.");
            return;
        }

        vault.Credentials.Add(new Credential
        {
            Site = site.Trim(),
            Username = username.Trim(),
            Password = password
        });

        ShowMessage("Credential added.");
    }

    private void ListCredentialsFlow(VaultData vault)
    {
        Console.Clear();
        Console.WriteLine("Stored credentials:");

        if (vault.Credentials.Count == 0)
        {
            Console.WriteLine("No credentials stored.");
        }
        else
        {
            for (int i = 0; i < vault.Credentials.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {vault.Credentials[i].Site}");
            }
        }
        Pause();
    }
    
    private void ShowCredentialFlow(VaultData vault)
    {
        Console.Clear();
        Console.WriteLine("Show credential");
        Console.Write("Enter site name: ");
        string? site = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(site))
        {
            ShowMessage("Site name cannot be empty.");
            return;
        }

        Credential? credential = vault.Credentials
            .FirstOrDefault(c => c.Site.Equals(site.Trim(), StringComparison.OrdinalIgnoreCase));

        if (credential is null)
        {
            ShowMessage("Credential not found.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"Site: {credential.Site}");
        Console.WriteLine($"Username: {credential.Username}");
        Console.WriteLine($"Password: {credential.Password}");
        Pause();
    }
    
    private void RemoveCredentialFlow(VaultData vault)
    {
        Console.Clear();
        Console.WriteLine("Remove credential");
        Console.Write("Enter site name: ");
        string? site = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(site))
        {
            ShowMessage("Site name cannot be empty.");
            return;
        }

        Credential? credential = vault.Credentials
            .FirstOrDefault(c => c.Site.Equals(site.Trim(), StringComparison.OrdinalIgnoreCase));

        if (credential is null)
        {
            ShowMessage("Credential not found.");
            return;
        }

        vault.Credentials.Remove(credential);
        ShowMessage("Credential removed.");   
    }
    
    private void GeneratePasswordFlow()
    {
        Console.Clear();
        Console.WriteLine("Generate password");
        Console.Write("Enter desired password length (minimum 12, default 20): ");

        string? input = Console.ReadLine();
        int length = 20;

        if (!string.IsNullOrWhiteSpace(input))
        {
            if (!int.TryParse(input, out length))
            {
                ShowMessage("Invalid number.");
                return;
            }
        }

        try
        {
            string password = _passwordGeneratorService.GeneratePassword(length);

            Console.WriteLine();
            Console.WriteLine("Generated password:");
            Console.WriteLine(password);
            Pause();
        }
        catch (Exception ex)
        {
            ShowMessage($"Error: {ex.Message}");
        }
    }
    
    private void ShowMessage(string message)
    {
        Console.WriteLine();
        Console.WriteLine(message);
        Pause();
    }

    private void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
    
}