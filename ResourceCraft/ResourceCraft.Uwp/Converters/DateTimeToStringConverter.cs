using System;
using Windows.UI.Xaml.Data;

namespace ResourceCraft.Uwp.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is DateTime dateTime))
            {
#if DEBUG
                return $"DateToStringConverter: Invalid object type supplied.";
#else
                return "";
#endif
            }

            if (!(parameter is string format) || string.IsNullOrWhiteSpace(format))
            {
                format = "MMMMM dd yyyy";
            }

            try
            {
                return DateTime.Now.ToString(format);
            }
            catch (Exception)
            {
#if DEBUG
                return $"DateToStringConverter: Invalid format supplied.";
#else
                return "";
#endif
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}