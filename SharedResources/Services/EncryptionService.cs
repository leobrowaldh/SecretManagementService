using SharedResources.Services;
using System;
using System.Security.Cryptography;
using System.Text;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key; // The key used for encryption/decryption
    private readonly int _tagSizeInBytes; // The size of the authentication tag in bytes

    public EncryptionService(byte[] key, int tagSizeInBytes = 16)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (tagSizeInBytes < 12 || tagSizeInBytes > 16)
            throw new ArgumentException("Tag size must be between 12 and 16 bytes.", nameof(tagSizeInBytes));

        _key = key;
        _tagSizeInBytes = tagSizeInBytes;
    }

    // Encrypt a plaintext string using AES-GCM
    public string Encrypt(string plaintext)
    {
        using var aes = new AesGcm(_key, _tagSizeInBytes);

        // Generate a random IV for each encryption (96 bits = 12 bytes)
        byte[] iv = new byte[12];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(iv);
        }

        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[_tagSizeInBytes];

        aes.Encrypt(iv, plaintextBytes, ciphertext, tag);

        // Concatenate IV, ciphertext, and tag into a single byte array
        var result = new byte[iv.Length + ciphertext.Length + tag.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(ciphertext, 0, result, iv.Length, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, result, iv.Length + ciphertext.Length, tag.Length);

        return Convert.ToBase64String(result);
    }

    // Decrypt an AES-GCM ciphertext string
    public string Decrypt(string ciphertext)
    {
        var encryptedData = Convert.FromBase64String(ciphertext);
        var iv = new byte[12]; // 96-bit IV for AES-GCM
        var tag = new byte[_tagSizeInBytes];
        var ciphertextBytes = new byte[encryptedData.Length - iv.Length - tag.Length];

        Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(encryptedData, iv.Length + ciphertextBytes.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(encryptedData, iv.Length, ciphertextBytes, 0, ciphertextBytes.Length);

        using var aes = new AesGcm(_key, _tagSizeInBytes);
        var plaintext = new byte[ciphertextBytes.Length];
        aes.Decrypt(iv, ciphertextBytes, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }

    // Mask the email or phone number with stars
    public string MaskSensitiveData(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        // If it's an email address
        if (value.Contains('@'))
        {
            var parts = value.Split('@');
            var localPart = parts[0];
            var domainPart = parts[1];
            var maskedLocal = $"{localPart.Substring(0, 2)}****{localPart.Substring(localPart.Length - 2)}";
            return $"{maskedLocal}@{domainPart}";
        }

        // If it's a phone number (simplified case: 10 digits)
        if (value.Length == 10 && long.TryParse(value, out _))
        {
            return $"{value.Substring(0, 2)}****{value.Substring(value.Length - 2)}";
        }

        // If it's neither, just return the original value
        return value;
    }
}

