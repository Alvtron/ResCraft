using ResourceCraft.Model;
using System;
using Windows.UI.Xaml.Data;

namespace ResourceCraft.Uwp.Converters
{
    public class ExperienceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case null:
                    return null;
                case int exp:
                    return Experience.ProgressExpInPercentage(exp);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
