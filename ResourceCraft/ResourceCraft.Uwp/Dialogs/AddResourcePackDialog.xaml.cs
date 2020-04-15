using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.ViewModels;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Dialogs
{
    public sealed partial class AddResourcePackDialog : ContentDialog
    {
        private AddResourcePackViewModel ViewModel { get; }

        public AddResourcePackDialog()
        {
            if (AuthService.CurrentUser == null)
            {
                Hide();
                return;
            }

            ViewModel = new AddResourcePackViewModel();
            InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (await ViewModel.UploadResourcePackAsync())
            {
                Hide();
            }
        }
    }
}
