using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using ResourceCraft.Uwp.Utilities;

namespace ResourceCraft.Uwp.Converters
{
    public class ImageConverterr : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case null:
                    break;
                case byte[] bytes:
                    return StorageUtilities.ConvertByteArrayToBitmapImage(bytes);
                case Uri uri:
                    return new BitmapImage(uri);
                case Image image:
                    return image.Source;
            }

            if (parameter is string parameterString && parameterString.ToUpper() == "SHOW_NO_IMAGE")
                return StorageUtilities.ConvertFileToBitmapImage(@"Assets\no_image_available.jpg");

            return null;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
