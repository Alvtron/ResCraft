using ResourceCraft.Model;
using ResourceCraft.DataAccess;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ResourceCraft.Uwp.Utilities
{
    public class ImageManager<T> : ObservableObject where T : FirebaseImage
    {
        private RelayCommand<IList<object>> _downloadImagesCommand;
        public ICommand DownloadImagesCommand => _downloadImagesCommand = _downloadImagesCommand ?? new RelayCommand<IList<object>>(async images => await DownloadImagesAsync(images));

        private RelayCommand<IList<object>> _deleteImagesCommand;
        public ICommand DeleteImagesCommand => _deleteImagesCommand = _deleteImagesCommand ?? new RelayCommand<IList<object>>(async images => await DeleteImagesAsync(images));

        public ObservableCollection<T> Images { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetField(ref _isBusy, value);
        }

        public ImageManager(ObservableCollection<T> images)
        {
            Images = images;
        }

        private async Task DownloadImagesAsync(IList<object> objects)
        {
            if (objects == null)
            {
                return;
            }

            IsBusy = true;

            var images = objects.Cast<ProfilePicture>();

            var folder = await StorageUtilities.PickFolderDestination();

            if (folder == null)
            {
                return;
            }

            foreach (var image in images)
            {
                await StorageUtilities.SaveFirebaseFileToStorageFolder(image, "images", folder);
            }

            IsBusy = false;
        }

        private async Task DeleteImagesAsync(IList<object> objects)
        {
            if (objects == null)
            {
                return;
            }

            IsBusy = true;

            var images = objects.Cast<T>();

            foreach (var image in images)
            {
                Images.Remove(image);
                await StorageSource.DeleteFileAsync($"images/{image.FileName}");
            }

            IsBusy = false;
        }
    }
}
