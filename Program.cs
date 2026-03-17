using PasswordManager.Services;

var keyDerivationService = new KeyDerivationService();
var encryptionService = new EncryptionService();
var vaultService = new VaultService(keyDerivationService, encryptionService);
var consoleUiService = new ConsoleUiService(vaultService);

consoleUiService.Run();