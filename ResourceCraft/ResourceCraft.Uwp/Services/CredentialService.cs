using System.Collections.Generic;
using System.Linq;
using Windows.Security.Credentials;
using System.Collections.ObjectModel;
using ResourceCraft.Utilities;
using System.Runtime.InteropServices;

namespace ResourceCraft.Uwp.Services
{
    /// <summary>
    /// 
    /// </summary>
    public static class CredentialService
    {
        public enum Response
        {
            Exist,
            NotFound,
            Added,
            Removed,
            NotSignedIn,
            SignedIn
        }

        /// <summary>
        /// The resource name
        /// </summary>
        private const string RESOURCE_NAME = "ResourceCraft";

        /// <summary>
        /// The vault
        /// </summary>
        private static readonly PasswordVault Vault = new PasswordVault();

        /// <summary>
        /// Gets the current credential.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static PasswordCredential Current
        {
            get
            {
                IReadOnlyList<PasswordCredential> credentialList;

                try
                {
                    credentialList = Vault.FindAllByResource(RESOURCE_NAME);
                }
                catch (COMException)
                {
                    // No credentials found with the resource name.
                    return null;
                }
                catch(System.Exception)
                {
                    // No credentials found with the resource name.
                    return null;
                }

                switch (credentialList.Count)
                {
                    case 0:
                        // There's no credentials stored in the vault.
                        return null;
                    case 1:
                        // There's exaclty one credential stored in the vault.
                        credentialList[0].RetrievePassword();
                        return credentialList[0];
                }

                // There are multiple credentials. Check if there is a default credential.
                if (!string.IsNullOrWhiteSpace(AppSettings.DefaultUser))
                {
                    var credential = Vault.Retrieve(RESOURCE_NAME, AppSettings.DefaultUser);
                    credential.RetrievePassword();
                    return credential;
                }

                // TEMPORARY SOLUTION: Pick the first credential.
                credentialList[0].RetrievePassword();
                return credentialList[0];
            }
        }

        private static readonly AppSettings AppSettings = new AppSettings();

        /// <summary>
        /// Gets all credentials.
        /// </summary>
        /// <value>
        /// All.
        /// </value>
        public static ObservableCollection<PasswordCredential> All => new ObservableCollection<PasswordCredential>(Vault.RetrieveAll());

        /// <summary>
        /// The amount of credentials in the vault.
        /// </summary>
        /// <value>
        /// The amount of credentials in the vault.
        /// </value>
        public static int Count => Vault.RetrieveAll().Count;

        /// <summary>
        /// Gets a value indicating whether the password vault is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public static bool Empty => Count == 0;

        /// <summary>
        /// Checks if the specified credential is available.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <returns></returns>
        public static bool CheckIfAvailable(PasswordCredential credential) => All.Any(c => c.UserName.Equals(credential.UserName));

        /// <summary>
        /// Finds a credential that matches the specified user name.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns></returns>
        public static PasswordCredential Find(string userName) => All.FirstOrDefault(c => c.UserName.Equals(userName));

        /// <summary>
        /// Determines whether the vault containst the specified credential.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <returns>
        ///   <c>true</c> if the vault contains the specified credential; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(PasswordCredential credential)
        {
            if (Empty) return false;

            credential.RetrievePassword();

            foreach (var c in All)
            {
                c.RetrievePassword();
                if (credential.UserName.Equals(c.UserName) && credential.Password.Equals(c.Password))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Add a new credential to the vault.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static PasswordCredential Create(string userName, string password)
        {
            Logger.WriteLine($"Creating new credential with username {userName} and password {password}.");
            var credential = new PasswordCredential(RESOURCE_NAME, userName, password);

            if (Find(userName) is PasswordCredential existingCredential)
            {
                existingCredential.RetrievePassword();

                if (existingCredential.Password.Equals(password))
                {
                    Logger.WriteLine($"Credential {credential.UserName} already exists. No need to add to vault.");
                    AppSettings.DefaultUser = credential.UserName;
                    return credential;
                }

                Logger.WriteLine($"Credential {credential.UserName} already exists, but with a different password. Proceeds to update it with the new password.");
                Delete(existingCredential);
            }
            
            Vault.Add(credential);
            Logger.WriteLine($"Credential {credential.UserName} was added to the vault.");
            AppSettings.DefaultUser = credential.UserName;
            Logger.WriteLine($"Credential {credential.UserName} was set as Default.");

            return credential;
        }

        /// <summary>
        /// Deletes the a password credential that matches the specified user name and password.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static Response Delete(string userName, string password)
        {
            return Delete(new PasswordCredential(RESOURCE_NAME, userName, password));
        }

        /// <summary>
        /// Deletes the specified credential.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <returns></returns>
        public static Response Delete(PasswordCredential credential)
        {
            if (!Contains(credential))
            {
                return Response.NotFound;
            }

            Vault.Remove(credential);

            if (credential.UserName.Equals(AppSettings.DefaultUser))
            {
                AppSettings.DefaultUser = "";
            }

            return Response.Removed;
        }

        /// <summary>
        /// Deletes all stored credentials in the vault.
        /// </summary>
        public static void DeleteAll()
        {
            foreach (var credential in Vault.RetrieveAll())
            {
                Vault.Remove(credential);
            }
        }
    }
}
