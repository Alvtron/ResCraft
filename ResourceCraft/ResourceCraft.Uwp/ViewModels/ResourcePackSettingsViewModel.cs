using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using ResourceCraft.Uwp.Dialogs;
using ResourceCraft.Utilities;
using ResourceCraft.DataAccess;
using ResourceCraft.Uwp.ViewModels;

namespace ResourceCraft.Uwp.ViewModels
{
    public class ResourcePackSettingsViewModel : ModelSettingsViewModel<ResourcePack>
    {
        private RelayCommand _uploadScreenshotsCommand;
        public ICommand UploadScreenshotsCommand => _uploadScreenshotsCommand = _uploadScreenshotsCommand ?? new RelayCommand(async param => await UploadScreenshotsAsync());

        private RelayCommand<Screenshot> _deleteScreenshotCommand;
        public ICommand DeleteScreenshotCommand => _deleteScreenshotCommand = _deleteScreenshotCommand ?? new RelayCommand<Screenshot>(screenshot => DeleteScreenshot(screenshot));

        private RelayCommand _uploadVideoCommand;
        public ICommand UploadVideoCommand => _uploadVideoCommand = _uploadVideoCommand ?? new RelayCommand(async param => await UploadVideoAsync());

        private RelayCommand<Video> _deleteVideoCommand;
        public ICommand DeleteVideoCommand => _deleteVideoCommand = _deleteVideoCommand ?? new RelayCommand<Video>(param => DeleteVideo(param));

        public ResourcePackSettingsViewModel(ResourcePack resourcePack)
            : base(resourcePack)
        {

        }

        public async Task UploadScreenshotsAsync()
        {
            var imageFiles = await StorageUtilities.PickMultipleImages();
            if (imageFiles == null || imageFiles.Count == 0) return;

            NavigationService.Lock();

            foreach (var imageFile in imageFiles)
            {
                Model?.AddScreenshot(AuthService.CurrentUser, await ImageUtilities.CreateNewImageAsync<Screenshot>(imageFile));
            }

            NavigationService.Unlock();

            IsModelChanged = true;
        }

        public void DeleteScreenshot(Screenshot screenshot)
        {
            if (screenshot == null)
            {
                return;
            }

            Model.Screenshots.Remove(screenshot);
        }

        public async Task UploadVideoAsync()
        {
            var dialog = new AddVideoDialog();
            NavigationService.Lock();

            if (await dialog.ShowAsync() != ContentDialogResult.Secondary || dialog.Video == null)
            {
                await NotificationService.DisplayErrorMessage("Video was invalid");
                NavigationService.Unlock();
                return;
            }

            Model?.AddVideo(AuthService.CurrentUser, dialog.Video);
            NavigationService.Unlock();

            IsModelChanged = true;
        }

        public void DeleteVideo(Video video)
        {
            if (video == null)
            {
                return;
            }

            Model?.Videos.Remove(video);
        }
    }
}