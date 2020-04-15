using ResourceCraft.Model;
using ResourceCraft.Uwp.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Dialogs
{
    public sealed partial class UploadImageDialog : ContentDialog
    {
        private StorageFile ImageFile { get; set; }

        public UploadImageDialog()
        {
            InitializeComponent();
        }

        private void UploadButton_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void UploadButton_Drop(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                return;
            }

            var items = await e.DataView.GetStorageItemsAsync();

            if (!(items.FirstOrDefault() is StorageFile imageFile))
            {
                return;
            }
            
            ImageFile = imageFile;

            Hide();
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            var imageFile = await StorageUtilities.PickSingleImage();

            if (imageFile == null)
            {
                return;
            }

            ImageFile = imageFile;

            Hide();
        }

        public async Task<T> CreateImageFromFile<T>() where T : FirebaseImage, new()
        {
            return await ImageUtilities.CreateNewImageAsync<T>(ImageFile);
        }
    }
}
