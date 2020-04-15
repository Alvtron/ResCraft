using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using ResourceCraft.Model;
using System.Net.Http.Headers;
using ResourceCraft.Utilities;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.Generic;
using ResourceCraft.DataAccess.Properties;

namespace ResourceCraft.DataAccess
{

    public static class RestApiService
    {
        private static FirebaseClient _client;
        public static FirebaseClient Client
        {
            get
            {
                if (_client != null)
                {
                    return _client;
                }

                Logger.WriteLine($"Initializing Firebase Rest Api...");

                return _client = new FirebaseClient(
                    Resources.FIREBASE_BASE_PATH,
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(Resources.FIREBASE_DATABASE_SECRET)
                    });
            }
        }
    }

    public static class RestApiService<T> where T : FirebaseEntity, new()
    {
        private static string Type => typeof(T).Name;

        public static async Task<List<T>> Get(int? limitToFirst = null)
        {
            Logger.WriteLine($"Retrieving all {Type}s from the database...");

            var query = RestApiService.Client.Child(Type).OrderByKey();

            var result = new List<T>();

            if (limitToFirst.HasValue)
            {
                foreach (var item in await query.LimitToFirst(limitToFirst.Value).OnceAsync<T>())
                {
                    result.Add(item.Object);
                }
            }
            else
            {
                foreach (var item in await query.OnceAsync<T>())
                {
                    result.Add(item.Object);
                }
            }

            return result;
        }

        public static async Task<T> Get(FirebaseKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.Uid) || string.IsNullOrWhiteSpace(key.Group))
            {
                Logger.WriteLine($"Provided entity key was empty.");
                throw new ArgumentNullException(nameof(key), "Provided entity key was empty");
            }

            Logger.WriteLine($"Retrieving {Type} at {key.Group}/{key.Uid} from Firebase Database...");

            var item = await RestApiService.Client
                .Child(key.Group)
                .Child(key.Uid)
                .OnceSingleAsync<T>();

            return item;
        }

        public static async Task<bool> Update(T item)
        {
            if (item == null)
            {
                Logger.WriteLine($"Provided {Type} item was empty.");
                throw new ArgumentNullException(nameof(item), $"Provided {Type} item was empty.");
            }

            if (item.Key == null || string.IsNullOrWhiteSpace(item.Key.Uid) || string.IsNullOrWhiteSpace(item.Key.Group))
            {
                Logger.WriteLine($"Provided entity key was empty.");
                throw new ArgumentNullException(nameof(item.Key), "Provided entity key was empty");
            }

            Logger.WriteLine($"Updating {Type} in the database...");

            await RestApiService.Client
                .Child(item.Key.Group)
                .Child(item.Key.Uid)
                .PutAsync(item);

            Logger.WriteLine($"{Type} was successfully updated in the database.");

            return true;
        }

        public static async Task<bool> Add(T item)
        {
            if (item == null)
            {
                Logger.WriteLine($"Provided {Type} item was empty.");
                throw new ArgumentNullException(nameof(item), $"Provided {Type} item was empty.");
            }

            if (item.Key == null || string.IsNullOrWhiteSpace(item.Key.Uid) || string.IsNullOrWhiteSpace(item.Key.Group))
            {
                Logger.WriteLine($"Provided entity key was empty.");
                throw new ArgumentNullException(nameof(item.Key), "Provided entity key was empty");
            }

            Logger.WriteLine($"Adding {Type} to the database...");

            await RestApiService.Client
                .Child(item.Key.Group)
                .Child(item.Key.Uid)
                .PutAsync(item);

            Logger.WriteLine($"{Type} was successfully added to the database at UID.");

            return true;
        }

        public static async Task<bool> Delete(FirebaseKey key)
        {
            if (key == null || string.IsNullOrWhiteSpace(key.Uid) || string.IsNullOrWhiteSpace(key.Group))
            {
                Logger.WriteLine($"Provided entity was empty.");
                throw new ArgumentNullException(nameof(key), "Provided entity was empty");
            }

            Logger.WriteLine($"Deleting {Type} from the database...");

            await RestApiService.Client
                .Child(key.Group)
                .Child(key.Uid)
                .DeleteAsync();

            Logger.WriteLine($"{Type} was successfully deleted from the database.");

            return true;
        }
    }
}