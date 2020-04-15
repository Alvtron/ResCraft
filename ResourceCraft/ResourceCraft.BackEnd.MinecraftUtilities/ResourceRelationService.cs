using Newtonsoft.Json;
using ResourceCraft.BackEnd.MinecraftUtilities;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ResourceCraft.BackEnd.MinecraftUtilities
{
    public class ResourceInfo : IResource
    {
        public string Version { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public long Bytes { get; set; }

        public ResourceInfo(string version, string path, string fullPath, long bytes)
        {
            Version = version;
            Path = path;
            FullPath = fullPath;
            Bytes = bytes;
        }
    }

    public class ResourceRelationService
    {
        private const string FileName = "relations.json";
        private const string FirebaseStoragePath = "data/relations.json";
        private static List<ResourceRelation> Relations = new List<ResourceRelation>();

        public static async Task CreateAsync()
        {
            Logger.WriteLine($"Creating relations for all versions...");

            await MinecraftService.InitializeAsync();
            await MinecraftService.DownloadAllAsync();

            var versions = MinecraftAPI.Versions.OrderByDescending(v => v.ReleaseTime).Select(x => x.Id).ToList();
            var fileDictionary = new Dictionary<string, Dictionary<string, ResourceInfo>>();

            for (var i = 0; i < versions.Count; i++)
            {
                var folder = await MinecraftService.GetOrDownloadVersionFolder(versions[i]);
                var directory = new DirectoryInfo(folder.Path);
                var files = directory.EnumerateFiles("*.json", SearchOption.AllDirectories)
                    .Concat(directory.EnumerateFiles("*.png", SearchOption.AllDirectories))
                    .Concat(directory.EnumerateFiles("*.lang", SearchOption.AllDirectories));

                Debug.WriteLine($"({i + 1}/{versions.Count}): Preparing {files.Count()} resources from {versions[i]}.");

                fileDictionary.Add(versions[i], new Dictionary<string, ResourceInfo>());

                foreach (var file in files)
                {
                    // Trim awaway first '\' in file path.
                    var relativePath = file.FullName.Substring(folder.Path.Length).Remove(0, 1);
                    fileDictionary[versions[i]].Add(relativePath, new ResourceInfo(versions[i], relativePath, file.FullName, file.Length));
                }
            }

            Logger.WriteLine($"Creating new relation list.");
            Relations = new List<ResourceRelation>();

            for (var i = 0; i < versions.Count; i++)
            {
                Debug.WriteLine($"({i + 1}/{versions.Count}): Adding {versions[i]} to the relation set.");
                while (fileDictionary[versions[i]].Count != 0)
                {
                    var resource = fileDictionary[versions[i]].First().Value;

                    var relation = new ResourceRelation() { resource };
                    var previousResource = resource;
                    for (var j = i + 1; j < versions.Count; j++)
                    {
                        ResourceInfo match = null;
                        if (fileDictionary[versions[j]].ContainsKey(previousResource.Path))
                        {
                            // Matched with path
                            match = fileDictionary[versions[j]][previousResource.Path];
                        }
                        else
                        {
                            // No path matched. Check every hash.
                            var hash = CalculateMD5(previousResource.FullPath);

                            foreach (var r in fileDictionary[versions[j]].Values)
                            {
                                if (previousResource.Bytes == r.Bytes && hash.SequenceEqual(CalculateMD5(r.FullPath)))
                                {
                                    match = r;
                                    break;
                                }
                            }
                        }
                        if (match == null)
                        {
                            break;
                        }
                        relation.Add(match);
                        previousResource = match;
                    }
                    // Delete added resources from file dictionary.
                    foreach (var r in relation)
                    {
                        fileDictionary[r.Key].Remove(r.Value);
                    }

                    Relations.Add(relation);
                }
            }
            
            Logger.WriteLine($"{Relations.Count} relations were created.");
            Logger.WriteLine($"Saving them as JSON.");

            await SaveAsync();
        }

        public static async Task<List<ResourceRelation>> GetOrCreateAsync()
        {
            if (Relations?.Count == 0)
                await LoadAsync();
            if (Relations?.Count == 0)
                await CreateAsync();

            return Relations;
        }

        public static async Task SaveAsync()
        {
            var rootfolder = ApplicationData.Current.LocalFolder;
            var jsonFile = await rootfolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

            Logger.WriteLine($"Saving relations to temporary file at {jsonFile.Path}.");

            using (var file = new StreamWriter(jsonFile.Path))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, Relations);
            }

            Logger.WriteLine($"Uploading relations file to database at {FirebaseStoragePath}.");

            using (var stream = await jsonFile.OpenStreamForReadAsync())
            {
                var url = await StorageSource.UploadFileAsync(stream, FirebaseStoragePath);
                Logger.WriteLine($"Relations file was successfully uploaded to Firebase Storage at {url}.");
            }
        }

        public static async Task<bool> LoadAsync()
        {
            if (Relations?.Count != 0)
            {
                return true;
            }

            var fileUri = await StorageSource.GetFileUrlAsync(FirebaseStoragePath);

            if (fileUri == null)
            {
                return false;
            }
            
            var outputFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync(FileName);

            if (outputFile != null)
            {
                var json = File.ReadAllText(outputFile.Path);
                Relations = JsonConvert.DeserializeObject<List<ResourceRelation>>(json);
            }
            else
            {
                outputFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

                using (var webClient = new WebClient())
                {
                    var json = webClient.DownloadString(fileUri);
                    Relations = JsonConvert.DeserializeObject<List<ResourceRelation>>(json);
                }
            }
            

            return Relations?.Count != 0;
        }

        private static byte[] CalculateMD5(string filePath)
        {
            using (var md5 = MD5.Create())
            using (var fileStream = File.OpenRead(filePath))
            {
                return md5.ComputeHash(fileStream);
            }
        }
    }
}
