using Newtonsoft.Json;
using ResourceCraft.Model;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ResourceCraft.Utilities;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Diagnostics;
using System.Collections;
using ResourceCraft.DataAccess.Properties;

namespace ResourceCraft.DataAccess
{
    public static class AuthDataSource
    {
        public static FirebaseAuthLink FirebaseAuthLink { get; private set; }

        public static async Task<Model.User> SignIn(string email, string password)
        {
            Logger.WriteLine("Signing in user to database...");

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Resources.FIREBASE_WEB_API_KEY));

            try
            {
                FirebaseAuthLink = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch (FirebaseAuthException)
            {
                throw;
            }

            if (!FirebaseAuthLink.User.Email.Equals(email))
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrWhiteSpace(FirebaseAuthLink.FirebaseToken))
            {
                throw new ArgumentException();
            }

            if (FirebaseAuthLink.User == null)
            {
                Logger.WriteLine($"The sign in was unsuccessful.");
                return null;
            }

            Logger.WriteLine($"The sign in was successful.");

            var key = new FirebaseKey(FirebaseAuthLink.User.LocalId, typeof(Model.User).Name);
            var user = await RestApiService<Model.User>.Get(key);

            return user;
        }

        public static async Task<Model.User> SignUp(string username, string email, string password)
        {
            Logger.WriteLine("Signing up user to database...");

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Resources.FIREBASE_WEB_API_KEY));

            try
            {
                FirebaseAuthLink = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
                await FirebaseAuthLink.UpdateProfileAsync(username, null);
            }
            catch (FirebaseAuthException e)
            {
                Logger.WriteLine("An exception was thrown during sign up.");

                if (e.Reason == AuthErrorReason.EmailExists)
                {
                    Logger.WriteLine($"User {email} already exists. Attempting to sign in.");
                    return await SignIn(email, password);
                }
            }

            if (!FirebaseAuthLink.User.Email.Equals(email.ToLower()))
            {
                throw new ArgumentException();
            }

            if (FirebaseAuthLink.User == null)
            {
                Logger.WriteLine($"The sign up was unsuccessful.");
                return null;
            }

            Logger.WriteLine($"The sign up was successful.");

            var newUser = new Model.User
            {
                UserName = username,
                Key = new FirebaseKey(FirebaseAuthLink.User.LocalId, typeof(Model.User).Name)
            };

            await RestApiService<Model.User>.Add(newUser);

            return newUser;
        }
    }
}
