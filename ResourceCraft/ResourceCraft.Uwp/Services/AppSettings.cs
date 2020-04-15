using ResourceCraft.Model;
using ResourceCraft.Utilities;
using Windows.Storage;

/*
 * Source: https://docs.microsoft.com/en-us/windows/uwp/design/app-settings/store-and-retrieve-app-data
 * 
 * Here are data types you can use for app settings:
 * UInt8, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double
 * Boolean
 * Char16, String
 * DateTime, TimeSpan
 * GUID, Point, Size, Rect
 */

namespace ResourceCraft.Uwp.Services
{
    public sealed class AppSettings : ObservableObject
    {
        private static ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

        public bool StaySignedIn
        {
            get => ReadSettings(nameof(StaySignedIn), false);
            set
            {
                SaveSettings(nameof(StaySignedIn), value);
                NotifyPropertyChanged();
            }
        }

        public string DefaultUser
        {
            get => ReadSettings(nameof(DefaultUser), "");
            set
            {
                SaveSettings(nameof(DefaultUser), value);
                NotifyPropertyChanged();
            }
        }

        public static T ReadSettings<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values.ContainsKey(key))
                return (T)LocalSettings.Values[key];

            return defaultValue != null
                ? defaultValue
                : default(T);
        }

        public static void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }

        public static void DeleteSettings(string key)
        {
            LocalSettings.Values[key] = null;
        }

        public static void DeleteAllSettings()
        {
            DeleteSettings(nameof(StaySignedIn));
            DeleteSettings(nameof(DefaultUser));
        }

        public static void PrintSettings()
        {
            Logger.WriteLine("App settings stored:");

            foreach (var key in ApplicationData.Current.LocalSettings.Values)
            {
                Logger.WriteLine($"{key.Key}: {key.Value}");
            }
        }
    }
}
