using System;
using System.Security.Cryptography;
using System.Text;

namespace ResourceCraft.Utilities
{
    // Windows guide on encryption and decryption: https://docs.microsoft.com/en-us/dotnet/standard/security/walkthrough-creating-a-cryptographic-application

    public class AsymmetricEncryptor
    {
        private const int KeySize = 2048;

        private readonly string _privateKey;

        public RSAParameters PublicParameters { get; private set; }

        public AsymmetricEncryptor()
        {
            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                _privateKey = rsa.ToXmlString(true);
                PublicParameters = rsa.ExportParameters(false);
            }
        }

        public string Decrypt(string data)
        {
            var dataInBytes = Convert.FromBase64String(data);

            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                rsa.FromXmlString(_privateKey);

                var decryptedBytes = rsa.Decrypt(dataInBytes, false);

                return Base64Decode(Convert.ToBase64String(decryptedBytes));
            }
        }

        public static string Encrypt(string data, RSAParameters parameters)
        {
            var dataInBytes = Convert.FromBase64String(Base64Encode(data));

            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                rsa.ImportParameters(parameters);
                var encryptedBytes = rsa.Encrypt(dataInBytes, false);
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}