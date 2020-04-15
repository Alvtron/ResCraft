using Newtonsoft.Json;
using ResourceCraft.DataAccess;
using ResourceCraft.Minecraft;
using ResourceCraft.Model;
using ResourceCraft.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Web.Http;

namespace ResourceCraft.BackEnd.MinecraftUtilities
{
    public static class MinecraftService
    {
        public enum DownloadOption
        {
            Default,
            ExtractFiles
        }

        public static bool IsInitialized => MinecraftAPI.IsInitialized;

        public static async Task InitializeAsync()
        {
            Debug.WriteLine("Initializing MinecraftService...");

            var localDirectory = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MinecraftApi", CreationCollisionOption.OpenIfExists);

            if (!MinecraftAPI.IsInitialized)
            {
                await MinecraftAPI.InitializeAsync(localDirectory.Path);
            }

            Debug.WriteLine("...MinecraftService initialized.");
        }

        private static async Task<StorageFolder> GetOrCreateRootFolderAsync()
        {
            return await ApplicationData.Current.LocalFolder.CreateFolderAsync("Versions", CreationCollisionOption.OpenIfExists);
        }

        private static async Task<StorageFolder> GetOrCreateVersionFolder(string version)
        {
            StorageFolder root = await GetOrCreateRootFolderAsync();
            return await root.CreateFolderAsync(version, CreationCollisionOption.OpenIfExists);
        }

        public static async Task<StorageFile> DownloadAsync(string version, DownloadOption option = DownloadOption.Default)
        {
            if (!MinecraftAPI.IsInitialized)
            {
                Debug.WriteLine($"MinecraftService is not initialized. Proceeds to initialize it.");
                await InitializeAsync();
            }
            if (!MinecraftAPI.Contains(version))
            {
                Debug.WriteLine($"Version {version} does not exist in available links.");
                return null;
            }

            if (await ContainsZip(version))
            {
                Debug.WriteLine($"Version {version} is already downloaded. No further steps required.");
                return null;
            }

            var uri = new Uri(MinecraftAPI.Get(version)?.Downloads.Client?.Url);

            var root = await GetOrCreateRootFolderAsync();
            var file = await root?.CreateFileAsync(version + ".zip", CreationCollisionOption.ReplaceExisting);

            try
            {
                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation download = downloader?.CreateDownload(uri, file);
                Debug.WriteLine($"Downloading: {uri?.AbsolutePath}...");
                await download.StartAsync();
                Debug.WriteLine($"...Download completed: {uri?.AbsolutePath}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error when downloading {uri.AbsolutePath}:\n{e.Message}");
                return null;
            }

            await CreateResourceListAsync(version);

            if (option == DownloadOption.ExtractFiles)
                await Extract(version);

            return file;
        }

        public static async Task<bool> ContainsFolder(string version)
        {
            var root = await GetOrCreateRootFolderAsync();
            StorageFolder folder = null;
            try
            {
                folder = await root.GetFolderAsync(version);
            }
            catch(Exception)
            {
                return false;
            }

            return folder != null;
        }

        public static async Task<bool> ContainsZip(string version)
        {
            var root = await GetOrCreateRootFolderAsync();
            StorageFile file = null;
            try
            {
                file = await root.GetFileAsync(version + ".zip");
            }
            catch (Exception)
            {
                return false;
            }

            return file != null;
        }

        private static async Task Extract(string version, bool overwrite = true)
        {
            var folder = await GetOrCreateVersionFolder(version);
            var file = await GetOrDownloadVersionArchive(version);

            Debug.WriteLine($"Extracting {file?.Path}...");
            using (ZipArchive archive = ZipFile.OpenRead(file?.Path))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".class", StringComparison.OrdinalIgnoreCase)) continue;
                    if (entry.FullName.EndsWith(".mf", StringComparison.OrdinalIgnoreCase)) continue;
                    if (entry.FullName.EndsWith(".rsa", StringComparison.OrdinalIgnoreCase)) continue;
                    if (entry.FullName.EndsWith(".sf", StringComparison.OrdinalIgnoreCase)) continue;

                    var entryFileIInfo = new FileInfo(Path.Combine(folder?.Path, entry.FullName));
                    entryFileIInfo.Directory.Create();

                    try
                    {
                        entry.ExtractToFile(entryFileIInfo.FullName, overwrite);
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        Debug.WriteLine($"Could not extract {entryFileIInfo.Name} from {file.Name}. It's in a directory that does not exist.\n{e.Message}");
                    }
                }
            }

            Debug.WriteLine($"...Extraction completed: {file?.Path}");
        }

        public static async Task<StorageFile> GetOrDownloadVersionArchive(string version)
        {
            if(!await ContainsZip(version))
            {
                Debug.WriteLine($"Version {version} does not have a zip file. Proceeds to download it.");
                return await DownloadAsync(version);
            }

            StorageFolder folder = await GetOrCreateRootFolderAsync();
            return await folder.GetFileAsync(version + ".zip");
        }

        public static async Task<StorageFolder> GetOrDownloadVersionFolder(string version)
        {
            if (!await ContainsFolder(version))
            {
                Debug.WriteLine($"Version {version} does not have a folder. Checking if it has a Zip-file.");
                var archive = await GetOrDownloadVersionArchive(version);
                await Extract(version, true);
            }

            return await GetOrCreateVersionFolder(version);
        }

        public static async Task DownloadLatestAsync(DownloadOption option = DownloadOption.Default)
        {
            await DownloadAsync(MinecraftAPI.LatestRelease.Id, option);
        }

        public static async Task DownloadAllAsync(DownloadOption option = DownloadOption.Default)
        {
            foreach (Minecraft.Version version in MinecraftAPI.Versions)
                await DownloadAsync(version.Id, option);
        }

        private static async Task<StorageFile> CreateResourceListAsync(string version)
        {
            var folder = await GetOrCreateRootFolderAsync();
            var file = await GetOrDownloadVersionArchive(version);

            var stringList = new List<string>();

            using (ZipArchive archive = ZipFile.OpenRead(file?.Path))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".class", StringComparison.OrdinalIgnoreCase)) continue;
                    if (entry.FullName.EndsWith(".mf", StringComparison.OrdinalIgnoreCase)) continue;
                    if (entry.FullName.EndsWith(".rsa", StringComparison.OrdinalIgnoreCase)) continue;
                    if (entry.FullName.EndsWith(".sf", StringComparison.OrdinalIgnoreCase)) continue;

                    stringList?.Add(entry.FullName.Replace(@"/", @"\"));
                }
            }

            var textFile = await folder?.CreateFileAsync(Path.GetFileName(file?.DisplayName) + ".txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.AppendTextAsync(textFile, string.Join(Environment.NewLine, stringList));

            return textFile;
        }

        public static async Task<List<string>> GetOrCreateResourceListAsync(string version)
        {
            var root = await GetOrCreateRootFolderAsync();
            StorageFile file = null;
            try
            {
                file = await root?.GetFileAsync(version + ".txt");
            }
            catch (FileNotFoundException)
            {
                file = await CreateResourceListAsync(version);
            }

            var lines = await FileIO.ReadLinesAsync(file);

            return lines.ToList();
        }
    }
}
