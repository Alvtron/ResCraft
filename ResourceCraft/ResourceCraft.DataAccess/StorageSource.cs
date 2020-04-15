using Firebase.Storage;
using ResourceCraft.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ResourceCraft.DataAccess.Properties;

namespace ResourceCraft.DataAccess
{
    public static class StorageSource
    {
        public static FirebaseStorage FirebaseStorage
        {
            get
            {
                Logger.WriteLine($"Creating new instance of Firebase Storage...");

                if (AuthDataSource.FirebaseAuthLink != null)
                {
                    return new FirebaseStorage(
                        "resource-craft.appspot.com",
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(AuthDataSource.FirebaseAuthLink.FirebaseToken)
                        });
                }
                else
                {
                    return new FirebaseStorage(
                        "resource-craft.appspot.com",
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(Resources.FIREBASE_DATABASE_SECRET)
                        });
                }
            }
        }

        private static async Task<string> ExcecuteUploadTaskAsync(FirebaseStorageTask task, EventHandler<FirebaseStorageProgress> onProgressUpdate = null)
        {
            if (onProgressUpdate != null)
            {
                task.Progress.ProgressChanged += onProgressUpdate;
            }

            return await task;
        }

        public static async Task<string> UploadFileAsync(Stream stream, string path, EventHandler<FirebaseStorageProgress> onProgressUpdate = null)
        {
            Logger.WriteLine($"Uploading {path} ({stream.Length} bytes) from Firebase Storage.");

            try
            {
                var task = CreateStorageReference(path).PutAsync(stream);

                Logger.WriteLine($"{stream.Length} bytes successfully uploaded to Firebase Storage as {path}.");

                return await ExcecuteUploadTaskAsync(task, onProgressUpdate);
            }
            catch(Exception)
            {
                Logger.WriteLine($"Failed to upload file {path} to Firebase Storage.");
                throw;
            }
        }

        public static async Task<bool> DeleteFileAsync(string path)
        {
            Logger.WriteLine($"Deleting file {path} from Firebase Storage.");

            try
            {
                await CreateStorageReference(path).DeleteAsync();
                return true;
            }
            catch (Exception)
            {
                Logger.WriteLine($"Failed to delete file {path} from Firebase Storage.");
                throw;
            }
        }

        public static async Task<Uri> GetFileUrlAsync(string path)
        {
            Logger.WriteLine($"Retrieving Uri to {path} from Firebase Storage.");

            try
            {
                var urlString = await CreateStorageReference(path).GetDownloadUrlAsync();

                if (string.IsNullOrWhiteSpace(urlString))
                {
                    Logger.WriteLine($"Retrieved url string was empty.");
                    return null;
                }

                return new Uri(urlString);
            }
            catch(Exception)
            {
                Logger.WriteLine($"Failed to retrieve url at {path} from Firebase Storage.");
                throw;
            }
        }

        private static FirebaseStorageReference CreateStorageReference(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            try
            {
                var directoryPath = Path.GetDirectoryName(path);
                var directories = directoryPath.Split('\\');
                var fileName = Path.GetFileName(path);

                if (directories.Length > 0)
                {
                    var reference = FirebaseStorage.Child(directories[0]);
                    for (var index = 1; index < directories.Length; index++)
                    {
                        reference = reference.Child(directories[index]);
                    }
                    return reference.Child(fileName);
                }
                else
                {
                    return FirebaseStorage.Child(fileName);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
