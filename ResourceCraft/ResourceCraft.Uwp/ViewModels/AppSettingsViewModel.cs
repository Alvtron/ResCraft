using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;

namespace ResourceCraft.Uwp.ViewModels
{
    public class AppSettingsViewModel : ObservableObject
    {
        public AppSettings AppSettings { get; } = new AppSettings();
    }
}
