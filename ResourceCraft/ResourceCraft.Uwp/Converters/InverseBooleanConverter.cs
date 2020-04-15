using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ResourceCraft.Uwp.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolean)
            {
                return !boolean;
            }

            else throw new InvalidCastException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolean)
            {
                return !boolean;
            }

            else throw new InvalidCastException();
        }
    }
}
