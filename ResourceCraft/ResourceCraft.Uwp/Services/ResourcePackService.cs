using Newtonsoft.Json;
using ResourceCraft.BackEnd.MinecraftUtilities;
using ResourceCraft.DataAccess;
using ResourceCraft.Minecraft;
using ResourceCraft.Model;
using ResourceCraft.Utilities;
using ResourceCraft.Uwp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Search;
using Windows.UI.Xaml.Controls;

namespace ResourceCraft.Uwp.Services
{
    [JsonObject(MemberSerialization.OptIn)]
    public static class ResourcePackService
    {
        private const string MINECRAFT_DIRECTORY_TOKEN = "MinecraftDirectoryToken";
        private const string RESOURCE_PACKS_FOLDER_NAME = "resourcepacks";

        public static async Task<ResourcePack> CreateNewResourcePackAsync(StorageFile file, User author, string name, string description = null)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Debug.WriteLine($"Creating new resource pack from file {file.Path}.");

            var verison = await DetectVersion(file);

            var resourcePack = new ResourcePack(author, name, verison, description);

            using (var resourceStream = await file.OpenStreamForReadAsync())
            {
                if (await StorageSource.UploadFileAsync(await file.OpenStreamForReadAsync(), $"{RESOURCE_PACKS_FOLDER_NAME}/{resourcePack.FileName}") == null)
                {
                    Debug.WriteLine($"Resource Pack file {file.Path} failed to upload.");
                    return null;
                }
            }

            if (!await RestApiService<ResourcePack>.Add(resourcePack))
            {
                Debug.WriteLine($"Resource Pack {resourcePack.Key} failed to upload.");
                return null;
            }

            return resourcePack;
        }

        public static async Task<StorageFolder> GetMinecraftDirectoryAsync()
        {
            var token = await EnsureAccessToMinecraftDirectoryAsync();

            if (token == null)
            {
                return null;
            }

            return await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
        }

        public static async Task<StorageFolder> GetResourcePacksInstallFolderAsync()
        {
            var minecraftFolder = await GetMinecraftDirectoryAsync();
            return await minecraftFolder.CreateFolderAsync(RESOURCE_PACKS_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
        }

        public static async Task<string> EnsureAccessToMinecraftDirectoryAsync()
        {
            var token = AppSettings.ReadSettings(MINECRAFT_DIRECTORY_TOKEN, default(string));

            if (token != null)
            {
                return token;
            }

            return await RequestAccessToMinecraftFolder();
        }

        public static async Task<string> RequestAccessToMinecraftFolder()
        {
            var response = await NotificationService.DisplayConfirmationDialog("Permission request", "Allow access to your '.Minecraft' folder?");
            if (response == ContentDialogResult.None)
            {
                return null;
            }

            var folder = await StorageUtilities.PickComputerFolder();
            if (folder == null)
            {
                return null;
            }

            var token = StorageApplicationPermissions.FutureAccessList.Add(folder);
            AppSettings.SaveSettings(MINECRAFT_DIRECTORY_TOKEN, token);
            return token;
        }

        public static async Task<bool> ConfirmMinecraftFolder(StorageFolder folder)
        {
            return await folder.TryGetItemAsync(RESOURCE_PACKS_FOLDER_NAME) != null;
        }

        public static async Task DownloadToFile(ResourcePack resourcePack, Action<DownloadOperation> OnRangeDownloadedChange = null)
        {
            var downloader = new BackgroundDownloader();
            var outputFolder = await GetResourcePacksInstallFolderAsync() ?? KnownFolders.DocumentsLibrary;
            var outputFile = await outputFolder.CreateFileAsync(resourcePack.FileName, CreationCollisionOption.GenerateUniqueName);

            var progressCallback = new Progress<DownloadOperation>(OnRangeDownloadedChange);
            try
            {
                var source = await StorageSource.GetFileUrlAsync($"{RESOURCE_PACKS_FOLDER_NAME}/{resourcePack.FileName}");
                var d = downloader.CreateDownload(source, outputFile);
                d.Priority = BackgroundTransferPriority.Default;

                await d.StartAsync().AsTask(progressCallback);
            }
            catch(Exception exception)
            {
                Logger.WriteLine("Failed to download file.");
                Logger.WriteLine(exception.Message);
            }
        }

        private static async Task<StorageFile> PickResourcePackAsync()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads
            };
            picker.FileTypeFilter.Add(".zip");
            picker.FileTypeFilter.Add(".rar");

            return await picker.PickSingleFileAsync();
        }

        public static async Task<string> DetectVersion(StorageFile resourcePackFile, bool onlyReleases = true)
        {
            if (resourcePackFile == null)
            {
                throw new ArgumentNullException(nameof(resourcePackFile));
            }

            Logger.WriteLine($"Detecting resource pack version of file {resourcePackFile.Name}.");

            var relations = await ResourceRelationService.GetOrCreateAsync();
            if (relations?.Count == 0)
            {
                throw new InvalidOperationException("Retrieved relation set is empty.");
            }

            if (!MinecraftService.IsInitialized)
            {
                await MinecraftService.InitializeAsync();
            }
            var versionCount = onlyReleases ? MinecraftAPI.Releases.ToDictionary(x => x.Id, x => 0) : MinecraftAPI.Versions.ToDictionary(x => x.Id, x => 0);

            using (var stream = await resourcePackFile.OpenStreamForReadAsync())
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries.ToList())
                {
                    var entryPath = entry.FullName.Replace("/", "\\");
                    foreach (var relation in relations)
                    {
                        foreach (var versionPathPair in relation)
                        {
                            if (versionPathPair.Value.Equals(entryPath) && versionCount.ContainsKey(versionPathPair.Key))
                            {
                                versionCount[versionPathPair.Key]++;
                            }
                        }
                    }
                }
            }

            var maxCount = versionCount.Max(vp => vp.Value);
            var mostNominatedVersions = versionCount.Where(r => r.Value == maxCount).Select(vp => vp.Key);
            string maxVersion = null;
            foreach (var nominatedVersion in mostNominatedVersions)
            {
                if (maxVersion == null)
                {
                    maxVersion = nominatedVersion;
                }
                else
                {
                    maxVersion = MinecraftAPI.MaxVersion(maxVersion, nominatedVersion);
                }
            }
            return maxVersion;
        }

        public static async Task Convert(ResourcePack resourcePack, string version, bool quiet = false)
        {
            if (resourcePack == null)
            {
                throw new ArgumentNullException(nameof(resourcePack));
            }
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            var outputFolder = await StorageUtilities.PickFolderDestination();
            if (outputFolder == null)
            {
                // User decided to cancel. Return.
                return;
            }

            Logger.WriteLine($"Converting {resourcePack.Name} ({resourcePack.Version}) to Minecraft version {version}.");

            var temporaryOutputFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync($"{resourcePack.Name}_converted_temp", CreationCollisionOption.ReplaceExisting);

            var resourceFileUrl = await StorageSource.GetFileUrlAsync($"{RESOURCE_PACKS_FOLDER_NAME}/{resourcePack.FileName}");
            if (resourceFileUrl == null)
            {
                throw new InvalidOperationException("Could not retrieve resource pack file url.");
            }

            var relations = await ResourceRelationService.GetOrCreateAsync();
            if (relations?.Count == 0)
            {
                throw new InvalidOperationException("Retrieved relation set is empty.");
            }

            using (var webClient = new WebClient())
            using (var stream = new MemoryStream(webClient.DownloadData(resourceFileUrl)))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                var index = 0;
                foreach (var entry in archive.Entries.ToList())
                {
                    var entryPath = entry.FullName.Replace("/", "\\");
                    foreach (var relation in relations)
                    {
                        // Does it contain both versions?
                        if (!(relation.ContainsKey(resourcePack.Version) && relation.ContainsKey(version)))
                        {
                            continue;
                        }
                        // Does path match?
                        if (!relation[resourcePack.Version].Equals(entryPath))
                        {
                            continue;
                        }

                        await ExtractZipEntry(temporaryOutputFolder, entry, relation[version]);
                        break;
                    }

                    if (!quiet)
                    {
                        Logger.WriteLine($"{++index} of {archive.Entries.Count} converted.");
                    }
                }
            }

            // Move and rename temporary zip file.
            var temporaryMoveFilePAth = Path.Combine(ApplicationData.Current.LocalFolder.Path, $"{resourcePack.Name}_{version}.zip");
            ZipFile.CreateFromDirectory(temporaryOutputFolder.Path, temporaryMoveFilePAth);
            var temporaryMoveFile = await StorageFile.GetFileFromPathAsync(temporaryMoveFilePAth);
            await temporaryMoveFile.MoveAsync(outputFolder, $"{resourcePack.Name}_{version}.zip", NameCollisionOption.GenerateUniqueName);
            // Delete temporary files
            await temporaryOutputFolder.DeleteAsync();
        }

        private static async Task ExtractZipEntry(StorageFolder temporaryOutputFolder, ZipArchiveEntry entry, string relativePath)
        {
            var relativeFolderPath = Path.GetDirectoryName(relativePath);
            var filename = Path.GetFileName(relativePath);
            if (string.IsNullOrEmpty(relativeFolderPath))
            {
                var outputPath = Path.Combine(temporaryOutputFolder.Path, filename);
                entry.ExtractToFile(outputPath, true);
            }
            else
            {
                var folder = await temporaryOutputFolder.CreateFolderAsync(relativeFolderPath, CreationCollisionOption.OpenIfExists);
                var outputPath = Path.Combine(folder.Path, filename);
                entry.ExtractToFile(outputPath, true);
            }
        }
    }
}
