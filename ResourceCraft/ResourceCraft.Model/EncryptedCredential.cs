using ResourceCraft.Utilities;
using System.Security.Cryptography;

namespace ResourceCraft.Model
{
    public class EncryptedCredential
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public EncryptedCredential() { }

        public EncryptedCredential(string userName, string password, RSAParameters parameters)
        {
            UserName = AsymmetricEncryptor.Encrypt(userName, parameters);
            Password = AsymmetricEncryptor.Encrypt(password, parameters);
        }

        public bool Equals(EncryptedCredential credential)
        {
            return UserName.Equals(credential.UserName) && Password.Equals(credential.Password);
        }
    }
}
