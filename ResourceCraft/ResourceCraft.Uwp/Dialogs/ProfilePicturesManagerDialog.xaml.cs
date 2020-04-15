using ResourceCraft.Model;
using ResourceCraft.Uwp.Utilities;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Dialogs
{
    public sealed partial class ProfilePicturesManagerDialog : ContentDialog
    {
        private ImageManager<ProfilePicture> ImageManager { get; }

        public ProfilePicturesManagerDialog(ObservableCollection<ProfilePicture> profilePictures, string dialogTitle)
        {
            ImageManager = new ImageManager<ProfilePicture>(profilePictures);
            InitializeComponent();
            UpdateSelectedItemsCounter();
            Title = dialogTitle;
        }

        private void ImageGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedItemsCounter();
        }

        private void UpdateSelectedItemsCounter()
        {
            SelectedItemsText.Text = $"{ImageGridView.SelectedItems.Count}";
            SelectedItemsText.Text += ImageGridView.SelectedItems.Count > 0 ? " items selected" : " item selected";
        }
    }
}
