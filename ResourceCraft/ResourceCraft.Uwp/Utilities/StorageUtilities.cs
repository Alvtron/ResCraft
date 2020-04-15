using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ResourceCraft.DataAccess;
using ResourceCraft.Model;
using ResourceCraft.Utilities;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ResourceCraft.Uwp.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class StorageUtilities
    {
        public static StorageFolder AppFolder => ApplicationData.Current.LocalFolder;
        public static StorageFolder InstallationFolder => Windows.ApplicationModel.Package.Current.InstalledLocation;

        /// <summary>
        /// Gets the image picker.
        /// </summary>
        /// <value>
        /// The image picker.
        /// </value>
        public static Windows.Storage.Pickers.FileOpenPicker ImagePicker => new Windows.Storage.Pickers.FileOpenPicker
        {
            ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary,
            FileTypeFilter = { ".jpg", ".jpeg", ".png", ".gif" }
        };

        public static Windows.Storage.Pickers.FileOpenPicker FilePicker => new Windows.Storage.Pickers.FileOpenPicker
        {
            ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            FileTypeFilter = { "*" }
        };

        public static Windows.Storage.Pickers.FileSavePicker SaveZipPicker
        {
            get
            {
                var picker = new Windows.Storage.Pickers.FileSavePicker
                {
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
                    SuggestedFileName = "Archive"
                };
                picker.FileTypeChoices.Add("Zip", new List<string>() { ".zip" });

                return picker;
            }
        }

        public static Windows.Storage.Pickers.FileSavePicker SaveImagePicker
        {
            get
            {
                var picker = new Windows.Storage.Pickers.FileSavePicker
                {
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
                    SuggestedFileName = "file"
                };
                picker.FileTypeChoices.Add("Image", new List<string>() { ".jpg", ".png", ".jpeg", ".gif" });

                return picker;
            }
        }

        public static Windows.Storage.Pickers.FolderPicker FolderPicker
        {
            get
            {
                var picker = new Windows.Storage.Pickers.FolderPicker
                {
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop,
                };
                picker.FileTypeFilter.Add("*");

                return picker;
            }
        }

        public static Windows.Storage.Pickers.FolderPicker ComputerFolderPicker
        {
            get
            {
                var picker = new Windows.Storage.Pickers.FolderPicker
                {
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder,
                };
                picker.FileTypeFilter.Add("*");

                return picker;
            }
        }

        internal static async Task SaveFirebaseFileToStorageFolder(FirebaseFile file, string firebaseFolder, StorageFolder storageFolder)
        {
            StorageFile sampleFile = await storageFolder.CreateFileAsync(file.FileName, CreationCollisionOption.GenerateUniqueName);
            using (var client = new WebClient())
            {
                var url = await StorageSource.GetFileUrlAsync($"images/{file.FileName}");
                client.DownloadFileAsync(url, sampleFile.Path);
            }
        }

        public static async Task<IReadOnlyList<StorageFile>> PickMultipleImages() => await ImagePicker.PickMultipleFilesAsync();
        public static async Task<StorageFile> PickSingleImage() => await ImagePicker.PickSingleFileAsync();
        public static async Task<StorageFile> PickSingleFile() => await FilePicker.PickSingleFileAsync();
        public static async Task<IReadOnlyList<StorageFile>> PickMultipleFiles() => await FilePicker.PickMultipleFilesAsync();
        public static async Task<StorageFile> PickImageFileDestination() => await SaveImagePicker.PickSaveFileAsync();
        public static async Task<StorageFile> PickZipFileDestination() => await SaveZipPicker.PickSaveFileAsync();
        public static async Task<StorageFolder> PickFolderDestination() => await FolderPicker.PickSingleFolderAsync();
        public static async Task<StorageFolder> PickComputerFolder() => await ComputerFolderPicker.PickSingleFolderAsync();

        public static async Task<StorageFile> GetStorageFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Logger.WriteLine("GetStorageFile: Provided file path was empty.");
                return null;
            }

            try
            {
                return await StorageFile.GetFileFromPathAsync(filePath);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<StorageFolder> GetStorageFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                Logger.WriteLine("GetStorageFolder: Provided folder path was empty.");
                return null;
            }

            try
            {
                return await StorageFolder.GetFolderFromPathAsync(folderPath);
            }
            catch
            {
                return null;
            }
        }

        public static BitmapImage ConvertFileToBitmapImage(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Logger.WriteLine("ConvertFileToBitmapImage: Provided file path was empty.");
                return null;
            }

            var fileInBytes = ConvertFileToByteArray(filePath);

            return ConvertByteArrayToBitmapImage(fileInBytes);
        }

        /// <summary>
        /// Files to byte array.
        /// </summary>
        /// <param name="filePath">The filepath.</param>
        /// <returns></returns>
        public static byte[] ConvertFileToByteArray(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Logger.WriteLine("ConvertFileToByteArray: Provided path was empty.");
                return null;
            }

            using (var stream = System.IO.File.OpenRead($@"{Windows.ApplicationModel.Package.Current.InstalledLocation.Path}\{filePath}"))
            {
                var fileBytes = new byte[stream.Length];

                stream.Read(fileBytes, 0, fileBytes.Length);
                return fileBytes;
            }
        }

        public static string ConvertFileToString(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Logger.WriteLine("ConvertFileToString: Provided path was empty.");
                return null;
            }

            try
            {
                return System.IO.File.ReadAllText($@"{Windows.ApplicationModel.Package.Current.InstalledLocation.Path}\{filePath}");
            }
            catch (Exception e)
            {
                Logger.WriteLine($"ConvertFileToString: {e.Message}");
                return null;
            }
        }

        public static async Task<BitmapImage> ConvertStorageFileToBitmapImage(StorageFile file)
        {
            if (file == null)
            {
                Logger.WriteLine("ConvertStorageFileToBitmapImage: Provided storage file was empty.");
                return null;
            }

            BitmapImage bitmapImage = new BitmapImage();
            using (var stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read))
            {
                await bitmapImage.SetSourceAsync(stream);
            }

            return bitmapImage;
        }

        /// <summary>
        /// Convert from storage file to byte array.
        /// </summary>
        /// <param name="savedStorageFile">The saved storage file.</param>
        /// <returns></returns>
        public static async Task<byte[]> ConvertStorageFileToByteArrayAsync(StorageFile savedStorageFile)
        {
            using (var stream = await savedStorageFile.OpenStreamForReadAsync())
            {
                var bytes = new byte[(int)stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return bytes;
            }
        }

        /// <summary>
        /// Bitmaps the image to byte array.
        /// </summary>
        /// <param name="bitmapImage">The bitmap image.</param>
        /// <returns></returns>
        public static async Task<byte[]> ConvertBitmapImageToByteArrayAsync(BitmapImage bitmapImage)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(bitmapImage.UriSource);
            using (var inputStream = await file.OpenSequentialReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();

                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }

        /// <summary>
        /// Bytes the array to bitmap image.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static BitmapImage ConvertByteArrayToBitmapImage(byte[] bytes)
        {
            using (var ms = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(ms.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(bytes);
                    writer.StoreAsync().GetResults();
                }
                var image = new BitmapImage();
                image.SetSource(ms);
                return image;
            }
        }
    }
}
