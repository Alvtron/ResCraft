using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ResourceCraft.Uwp.Converters
{
    public class IsAuthorVisibilityConverter : IValueConverter
    {
        private const Visibility Visible = Visibility.Visible;
        private const Visibility Invisible = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is User user) return user.Key == AuthService.CurrentUser.Key ? Visible : Invisible;

            return value != null ? Visible : Invisible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}