using System.Threading.Tasks;

namespace ResourceCraft.Services
{
    public static class FtpService
    {
        private static readonly FtpClient Client = new FtpClient(RootDirectoryFtp, "r3dcraft.net", "#Alvtron1");

        public const string RootDirectoryHttp = @"https://r3dcraft.net/ResourceCraft";

        public const string RootDirectoryFtp = @"ftp://ftp.r3dcraft.net/ResourceCraft";

        public static bool Upload(byte[] file, string path) => Client.Upload(file, path);

        public static async Task<bool> UploadAsync(byte[] file, string path) => await Client.UploadAsync(file, path);

        public static byte[] Download(string path) => Client.Download(path);

        public static async Task<byte[]> DownloadAsync(string path) => await Client.DownloadAsync(path);

        public static bool Delete(string path) => Client.Delete(path);

        public static async Task<bool> DeleteAsync(string path) => await Client.DeleteAsync(path);

        public static bool DeleteAll() => Client.DeleteAll();

        public static async Task<bool> DeleteAllAsync() => await Client.DeleteAllAsync();

        public static bool Exist(string path) => Client.FileExists(path);

        public static async Task<bool> ExistAsync(string path) => await Client.FileExistsAsync(path);
    }
}
