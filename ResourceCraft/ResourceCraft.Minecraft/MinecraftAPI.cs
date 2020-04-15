using Newtonsoft.Json;
using ResourceCraft.DataAccess;
using ResourceCraft.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCraft.Minecraft
{
    public static class MinecraftAPI
    {
        private const string VERSION_FILE_NAME = "versions.json";

        private const string FIREBASE_STORAGE_PATH = "data/versions.json";

        private const string KEY_RELEASE = "release";

        private const string KEY_SNAPSHOT = "snapshot";

        private static HttpClient _httpClient = new HttpClient();

        private static readonly Uri _apiVersionsUri = new Uri(@"https://launchermeta.mojang.com/mc/game/version_manifest.json");

        public static string LocalDirectory { get; private set; } = string.Empty;

        public static string LocalVersionsFilePath => Path.Combine(LocalDirectory, VERSION_FILE_NAME);

        public static SortedSet<Version> Versions { get; private set; } = new SortedSet<Version>(new VersionComparer());

        public static DateTime Updated { get; private set; }

        public static bool IsInitialized => Versions?.Any() ?? false;

        public static List<string> Ids => Versions?.Select(v => v.Id)?.ToList();

        public static List<Version> Releases => Versions?.Where(v => v?.Type == KEY_RELEASE)?.ToList();

        public static List<Version> Snapshots => Versions?.Where(v => v?.Type == KEY_SNAPSHOT)?.ToList();

        public static Version Get(string version) => Versions?.FirstOrDefault(v => v?.Id == version);

        public static Version LatestRelease => Versions?.Where(v => v?.Type == KEY_RELEASE)?.OrderByDescending(v => v?.ReleaseTime)?.FirstOrDefault();

        public static Version LatestSnapshot => Versions?.Where(v => v?.Type == KEY_SNAPSHOT)?.OrderByDescending(v => v?.ReleaseTime)?.FirstOrDefault();

        public static bool Contains(string version) => Versions.Any(v => v?.Id == version);

        public static int CompareVersions(string firstVersion, string secondVersion) => Ids.IndexOf(secondVersion).CompareTo(Ids.IndexOf(firstVersion));

        public static string MaxVersion(string firstVersion, string secondVersion) => Ids.ElementAt(Math.Max(Ids.IndexOf(secondVersion), Ids.IndexOf(firstVersion)));

        public static List<string> VersionRange(string fromVersion, string toVersion)
        {
            var fromIndex = Ids.IndexOf(fromVersion);
            var toIndex = Ids.IndexOf(toVersion);
            return Ids.GetRange(fromIndex, toIndex - fromIndex);
        }

        public static async Task InitializeAsync(string localDirectoryPath)
        {
            Logger.WriteLine("Initializing Minecraft API.");

            LocalDirectory = localDirectoryPath ?? throw new ArgumentNullException(nameof(localDirectoryPath));

            foreach (var version in await DownloadNewVersionsAsync())
            {
                Versions.Add(version);
            }
            
            Logger.WriteLine("Saving versions.");
            await SaveVersionsToFirebaseStorage();
            SaveVersionsToFileAsync(LocalVersionsFilePath);

            Updated = DateTime.Now;
            Logger.WriteLine("Minecraft API was successfully initialized.");
        }

        private static async Task<IEnumerable<Version>> DownloadNewVersionsAsync()
        {
            if (Versions?.Count == 0)
            {
                await CreateVersionsFromFirebaseStorageAsync();
            }

            Logger.WriteLine("Downloading Version Manifest data.");
            var versionManifest = await DownloadVersionManifestAsync();
            var missingVersions = versionManifest.Versions.Where(v => !Versions.Any(v2 => v2.Id == v.Id));
            return await DownloadVersionsAsync(missingVersions);
        }

        private static async Task<VersionManifest> DownloadVersionManifestAsync()
        {
            string httpResponseBody;

            try
            {
                var httpResponse = await _httpClient.GetAsync(_apiVersionsUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }

            return JsonConvert.DeserializeObject<VersionManifest>(httpResponseBody);
        }

        private static async Task<List<Version>> DownloadVersionsAsync(IEnumerable<VersionMinimal> minimalVersions)
        {
            var versions = new List<Version>();

            foreach (var minimalVersion in minimalVersions)
            {
                Logger.WriteLine($"Downloading {minimalVersion.Id} data with {minimalVersion.Url}.");

                var httpResponse = new HttpResponseMessage();
                try
                {
                    httpResponse = await _httpClient.GetAsync(minimalVersion.Url);
                    httpResponse.EnsureSuccessStatusCode();
                    var httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                    versions.Add(JsonConvert.DeserializeObject<Version>(httpResponseBody));
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return versions;
        }

        private static bool CreateVersionsFromLocalJson(string localJsonFilePath)
        {
            Logger.WriteLine("Loading versions from Json file...");

            if (!File.Exists(localJsonFilePath))
            {
                throw new FileNotFoundException("File not found.", localJsonFilePath);
            }

            var json = File.ReadAllText(localJsonFilePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                Logger.WriteLine("... The local JSON-file is empty. Can't proceed.");
                return false;
            }

            Versions = JsonConvert.DeserializeObject<SortedSet<Version>>(json);

            Logger.WriteLine("...Versions was loaded from Json file.");
            return true;
        }

        private static async Task<bool> CreateVersionsFromFirebaseStorageAsync(string localJsonSaveFilePath = null)
        {
            Logger.WriteLine("Loading versions from Firebase Storage...");

            var fileUri = await StorageSource.GetFileUrlAsync(FIREBASE_STORAGE_PATH);

            if (fileUri == null)
            {
                throw new FileNotFoundException("File not found.", FIREBASE_STORAGE_PATH);
            }

            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString(fileUri);
                if (string.IsNullOrWhiteSpace(json))
                {
                    Logger.WriteLine("The local JSON-file is empty. Can't proceed.");
                    return false;
                }

                Versions = JsonConvert.DeserializeObject<SortedSet<Version>>(json);

                if (!string.IsNullOrEmpty(localJsonSaveFilePath))
                {
                    SaveVersionsToFileAsync(localJsonSaveFilePath);
                }

                return true;
            }
        }

        private static void SaveVersionsToFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            Logger.WriteLine($"Creating and deserializing JSON-string to local JSON-file at {filePath}.");

            using (var file = new StreamWriter(filePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, Versions);
            }
        }

        private static async Task SaveVersionsToFirebaseStorage()
        {
            Logger.WriteLine($"Uploading versions to database at {FIREBASE_STORAGE_PATH}.");
            var json = JsonConvert.SerializeObject(Versions);
            byte[] jsonByteArray = Encoding.UTF8.GetBytes(json);
            using (var stream = new MemoryStream(jsonByteArray))
            {
                var url = await StorageSource.UploadFileAsync(stream, FIREBASE_STORAGE_PATH);
                Logger.WriteLine($"Relations file was successfully uploaded to Firebase Storage at {url}.");
            }
        }
    }
}
