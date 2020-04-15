using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ResourceCraft.Uwp.Converters
{
    public class InverseVisibilityConverter : IValueConverter
    {
        private const Visibility Visible = Visibility.Visible;
        private const Visibility Invisible = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isVisible) return (isVisible) ? Invisible : Visible;

            return value != null ? Invisible : Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case Visible: return true;
                case Invisible: return false;
            }

            return false;
        }
    }
}
