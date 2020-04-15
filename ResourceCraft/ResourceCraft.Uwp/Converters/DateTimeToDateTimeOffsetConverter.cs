using System;
using Windows.UI.Xaml.Data;

namespace ResourceCraft.Uwp.Converters
{
    public class DateTimeToDateTimeOffsetConverter : IValueConverter
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
                    return new DateTimeOffset(DateTime.Now);
                case DateTime dateTime:
                    return new DateTimeOffset(dateTime);
            }

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
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case null:
                    return null;
                case DateTimeOffset dateTimeOffset:
                    return dateTimeOffset.LocalDateTime;
            }

            return null;
        }
    }
}