using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Web.Application.Services.Interfaces;

namespace Web.Infrastructure
{
    public class EncryptionService : IEncryptionService
    {
        private readonly string _key;

        public EncryptionService(IConfiguration configuration)
        {
            _key = configuration["EncryptionSettings:Key"];
            if (string.IsNullOrWhiteSpace(_key) || _key.Length != 32)
            {
                throw new ArgumentException("Encryption key must be a 32-character string.");
            }
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentException("The text to encrypt cannot be null or empty.");
            }

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_key);
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var memoryStream = new MemoryStream())
                {
                    memoryStream.Write(aes.IV, 0, aes.IV.Length);
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cryptoStream))
                    {
                        writer.Write(plainText);
                    }
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentException("The text to decrypt cannot be null or empty.");
            }

            var cipherBytes = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_key);

                using (var memoryStream = new MemoryStream(cipherBytes))
                {
                    var iv = new byte[16];
                    memoryStream.Read(iv, 0, iv.Length);
                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(cryptoStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}