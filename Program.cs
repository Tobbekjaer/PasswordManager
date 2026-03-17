using PasswordManager.Services;

var keyDerivationService = new KeyDerivationService();
var encryptionService = new EncryptionService();
var vaultService = new VaultService(keyDerivationService, encryptionService);
var passwordGeneratorService = new PasswordGeneratorService();
var consoleUiService = new ConsoleUiService(vaultService, passwordGeneratorService);

consoleUiService.Run();