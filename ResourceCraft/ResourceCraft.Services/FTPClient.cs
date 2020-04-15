using ResourceCraft.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ResourceCraft.Services
{

    internal class FtpClient
    {
        public readonly string RootDirectory;
        private readonly NetworkCredential _credential;

        public FtpClient(string rootDirectory, string userName, string password)
        {
            RootDirectory = rootDirectory ?? throw new ArgumentNullException("Root directory cannot be null.");

            _credential = new NetworkCredential(userName, password);
        }

        private FtpWebRequest Request(string path) => WebRequest.Create(new Uri($"{RootDirectory}/{path}")) as FtpWebRequest;

        public async Task<byte[]> DownloadAsync(string path)
        {
            var request = Request(path);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = _credential;

            try
            {
                using (var response = await request.GetResponseAsync() as FtpWebResponse)
                {
                    using (var responseStream = response?.GetResponseStream())
                    {
                        if (responseStream == null)
                            return null;

                        Logger.WriteLine($"Download Complete. Status: {response.StatusDescription}");

                        using (var memoryStream = new MemoryStream())
                        {
                            await responseStream.CopyToAsync(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                }
            }
            catch(WebException)
            {
                return null;
            }
        }

        public byte[] Download(string path)
        {
            var request = Request(path);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = _credential;

            try
            {
                using (var response = request.GetResponse() as FtpWebResponse)
                {
                    using (var responseStream = response?.GetResponseStream())
                    {
                        if (responseStream == null)
                            return null;

                        Logger.WriteLine($"Download Complete. Status: {response.StatusDescription}");

                        using (var memoryStream = new MemoryStream())
                        {
                            responseStream.CopyTo(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                }
            }
            catch(WebException)
            {
                return null;
            }
        }

        public async Task<bool> UploadAsync(byte[] file, string path)
        {
            var request = Request(path);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = _credential;
            request.ContentLength = file.Length;

            using (var requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(file, 0, file.Length);
            }

            try
            {
                using (var response = await request.GetResponseAsync() as FtpWebResponse)
                {
                    Logger.WriteLine($"Upload File Complete. Status: {response?.StatusDescription}");
                    return true;
                }
            }
            catch (WebException)
            {
                Logger.WriteLine($"Upload file failed.");
                return false;
            }
}

        public bool Upload(byte[] file, string path)
        {
            var request = Request(path);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = _credential;
            request.ContentLength = file.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(file, 0, file.Length);
            }

            try
            {
                using (var response = request.GetResponse() as FtpWebResponse)
                {
                    Logger.WriteLine($"Upload File Complete. Status: {response?.StatusDescription}");
                    return true;
                }
            }
            catch (WebException)
            {
                Logger.WriteLine($"Upload file failed.");
                return false;
            }
        }

        public bool Delete(string path)
        {
            var request = Request(path);
            request.Credentials = _credential;
            request.Method = WebRequestMethods.Ftp.DeleteFile;

            try
            {
                using (var response = request.GetResponse() as FtpWebResponse)
                {
                    Logger.WriteLine($"Delete File Complete. Status: {response?.StatusDescription}");
                    return true;
                }
            }
            catch (WebException)
            {
                Logger.WriteLine($"Deletion of file failed.");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string path)
        {
            var request = Request(path);
            request.Credentials = _credential;
            request.Method = WebRequestMethods.Ftp.DeleteFile;

            try
            {
                using (var response = await request.GetResponseAsync() as FtpWebResponse)
                {
                    Logger.WriteLine($"Delete File Complete. Status: {response?.StatusDescription}");
                    return true;
                }
            }
            catch (WebException)
            {
                Logger.WriteLine($"Deletion of file failed.");
                return false;
            }
        }

        public bool DeleteAll()
        {
            var files = ListFiles();

            if (files.Count == 0)
                return true;

            bool success = false;

            foreach (var file in files)
                success = Delete(file);

            return success;
        }

        public async Task<bool> DeleteAllAsync()
        {
            var files = await ListFilesAsync();

            if (files.Count == 0)
                return true;

            bool success = false;

            foreach (var file in files)
                success = await DeleteAsync(file);

            return success;
        }

        public bool CreateDirectory(string path)
        {
            var request = Request(path);

            try
            {
                using (request.GetResponse() as FtpWebResponse)
                {
                    Logger.WriteLine($"Directory {request.RequestUri} already exists.");
                    return true;
                }
            }
            catch (WebException)
            {
                try
                {
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    using (request.GetResponse() as FtpWebResponse)
                    {
                        Logger.WriteLine($"Directory {request.RequestUri} was successfully created.");
                        return true;
                    }
                }
                catch (WebException)
                {
                    Logger.WriteLine($"Something wen't wrong when creating directory {request.RequestUri}.");
                    return false;
                }
            }
        }

        public async Task<bool> CreateDirectoryAsync(string path)
        {
            var request = Request(path);

            try
            {
                using (await request.GetResponseAsync() as FtpWebResponse)
                {
                    Logger.WriteLine($"Directory {request.RequestUri} already exists.");
                    return true;
                }
            }
            catch (WebException)
            {
                try
                {
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    using (await request.GetResponseAsync() as FtpWebResponse)
                    {
                        Logger.WriteLine($"Directory {request.RequestUri} was successfully created.");
                        return true;
                    }
                }
                catch (WebException)
                {
                    Logger.WriteLine($"Something wen't wrong when creating directory {request.RequestUri}.");
                    return false;
                }
            }
        }

        public List<string> ListFiles(string path = "")
        {
            var request = Request(path);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = _credential;

            using (var response = request.GetResponse() as FtpWebResponse)
            {
                if (response == null)
                    return null;

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var files = new List<string>();
                    var line = reader.ReadLine();
                    while (!string.IsNullOrEmpty(line))
                    {
                        files.Add(line);
                        line = reader.ReadLine();
                    }

                    Logger.WriteLine($"Directory List Complete, status {response.StatusDescription}");

                    return files;
                }
            }
        }

        public async Task<List<string>> ListFilesAsync(string path = "")
        {
            var request = Request(path);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = _credential;

            using (var response = await request.GetResponseAsync() as FtpWebResponse)
            {
                if (response == null)
                    return null;

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var files = new List<string>();
                    var line = await reader.ReadLineAsync();
                    while (!string.IsNullOrEmpty(line))
                    {
                        files.Add(line);
                        line = await reader.ReadLineAsync();
                    }

                    Logger.WriteLine($"Directory List Complete, status {response.StatusDescription}");

                    return files;
                }
            }
        }

        public bool FileExists(string path)
        {
            var request = Request(path);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.Credentials = _credential;

            try
            {
                using (request.GetResponse() as FtpWebResponse)
                {
                    return true;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }

        public async Task<bool> FileExistsAsync(string path)
        {
            var request = Request(path);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.Credentials = _credential;

            try
            {
                using (await request.GetResponseAsync() as FtpWebResponse)
                {
                    return true;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
