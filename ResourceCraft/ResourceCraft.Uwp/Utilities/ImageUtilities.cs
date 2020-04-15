using ResourceCraft.Model;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ResourceCraft.Utilities;
using ResourceCraft.DataAccess;

namespace ResourceCraft.Uwp.Utilities
{
    public class ImageUtilities
    {
        public static async Task<T> CreateNewImageAsync<T>(StorageFile storageFile, string description = "") where T : FirebaseImage, new()
        {
            Logger.WriteLine($"Creating new image file from storage file {storageFile.Path}...");

            using (var stream = await storageFile.OpenStreamForReadAsync())
            {
                var decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());

                var image = (T)Activator.CreateInstance(typeof(T), new object[]
                {
                    (int)decoder.PixelWidth,
                    (int)decoder.PixelHeight,
                    storageFile.FileType
                });

                await StorageSource.UploadFileAsync(stream, $"images/{image.FileName}");

                Logger.WriteLine($"Image from file {storageFile.Path} was successfully created.");

                return image;
            }
        }

        public static async Task<ImageSource> WebImageToCroppedImage<T>(T image) where T : FirebaseImage, ICroppableImage
        {
            // Convert start point and size to unsigned integer. 
            uint startPointX = (uint)image.Crop.X;
            uint startPointY = (uint)image.Crop.Y;
            uint height = (uint)image.Crop.Height;
            uint width = (uint)image.Crop.Width;

            var bitmap = new BitmapImage(await StorageSource.GetFileUrlAsync($"images/{image.FileName}"));
            var randomAccessStreamReference = RandomAccessStreamReference.CreateFromUri(bitmap.UriSour‌​ce);

            using (var stream = await randomAccessStreamReference.OpenReadAsync())
            {
                // Create a decoder from the stream. With the decoder, we can get the properties of the image. 
                var decoder = await BitmapDecoder.CreateAsync(stream);

                // Create the bitmap bounds
                var bitmapBounds = new BitmapBounds
                {
                    X = startPointX,
                    Y = startPointY,
                    Height = height,
                    Width = width
                };

                // Create cropping BitmapTransform. 
                var bitmapTransform = new BitmapTransform
                {
                    Bounds = bitmapBounds,
                    ScaledWidth = width,
                    ScaledHeight = height
                };

                // Get the cropped pixels within the bounds of transform. 
                PixelDataProvider pixelDataProvider = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    bitmapTransform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.ColorManageToSRgb);

                byte[] pixels = pixelDataProvider.DetachPixelData();

                // Stream the bytes into a WriteableBitmap 
                WriteableBitmap cropBmp = new WriteableBitmap((int)width, (int)height);
                Stream pixStream = cropBmp.PixelBuffer.AsStream();
                pixStream.Write(pixels, 0, (int)(width * height * 4));

                return cropBmp;
            }
        }
    }
}
