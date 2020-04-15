using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ResourceCraft.Uwp.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        private const Visibility Visible = Visibility.Visible;
        private const Visibility Invisible = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isVisible) return (isVisible) ? Visible : Invisible;

            return value != null ? Visible : Invisible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case Visible: return false;
                case Invisible: return true;
            }

            return false;
        }
    }
}