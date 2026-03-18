# Password Manager - Secure Software Development Mini Project

## Overview
This project implements a simple local password manager as part of the Secure Software Development course.

The application allows a user to securely store credentials in an encrypted vault protected by a master password. The vault is stored locally on disk and can only be accessed by deriving the correct encryption key from the master password.

---

## Features

- Create and open an encrypted vault
- Store credentials (site, username, password)
- AES-GCM encryption for secure storage
- Key derivation using PBKDF2 (HMAC-SHA256)
- Random salt and nonce generation using secure Cryptographic Random Number Generator
- Masked password input (no echo in console)
- Secure password generator
- Auto-lock after inactivity

---

## Security Design

### Key Derivation
The encryption key is derived from the master password using PBKDF2 with:

- HMAC-SHA256
- Random salt (16 bytes)
- Configurable iteration count (default: 200,000)

This ensures resistance against brute-force attacks.

### Encryption
The vault is encrypted using AES-GCM, which provides:

- Confidentiality (data cannot be read)
- Integrity (tampering is detected)

### Storage Format
The vault is stored as a JSON file containing:

- Salt
- Nonce
- Ciphertext
- Authentication tag
- Iteration count

Sensitive data is never stored in plaintext.

### Authentication
No password is stored. Instead:

- The user provides the master password
- The key is derived using PBKDF2
- If decryption succeeds, the password is correct

---

## How to Run

Requirements:
- .NET 10 SDK

Run the application:

```bash
dotnet run
```

## Usage

1. Create a vault (first time only)
2. Open the vault using your master password
3. Use Secure Password Generator to generate your passwords (optional)
4. Add, view, or remove credentials
6. Save and exit to persist changes

---

## Important Notes

- If the master password is incorrect, the vault cannot be decrypted
- If the vault file is corrupted or modified, decryption will fail
- Unsaved changes are lost if the application auto-locks

---

## GenAI Usage Disclosure

Parts of this project were developed with assistance from generative AI (ChatGPT).

AI was used for:

- Structuring the project architecture  
- Implementation of PasswordGeneratorService.cs and SecureConsole.cs
- Providing explanations and best practices for secure software development  

All generated code was reviewed, understood, and adapted before inclusion in the final solution.

---

## Reflection

This project demonstrates practical use of applied cryptography, including key derivation, authenticated encryption, and secure handling of sensitive data in a local application.

