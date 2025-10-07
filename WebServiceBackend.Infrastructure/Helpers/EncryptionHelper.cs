using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using WebServiceBackend.Core.Options;

namespace WebServiceBackend.Infrastructure.Helpers
{
    public class EncryptionHelper
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionHelper(IOptions<EncryptionOptions> options)
        {
            var encryptionOptions = options.Value;

            if (string.IsNullOrWhiteSpace(encryptionOptions.Key) || encryptionOptions.Key.Length != 32)
                throw new ArgumentException("Encryption Key must be 32 characters long.");

            if (string.IsNullOrWhiteSpace(encryptionOptions.IV) || encryptionOptions.IV.Length != 16)
                throw new ArgumentException("Encryption IV must be 16 characters long.");

            _key = Encoding.UTF8.GetBytes(encryptionOptions.Key);
            _iv = Encoding.UTF8.GetBytes(encryptionOptions.IV);
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            var buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(buffer);
            using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }
    }
}
