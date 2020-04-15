using ResourceCraft.DataAccess;
using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using ResourceCraft.Uwp.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Networking.BackgroundTransfer;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class ResourcePackAuthorBlock : UserControl
    {
        public static readonly DependencyProperty ResourcePackProperty = DependencyProperty.Register("ResourcePack", typeof(ResourcePack), typeof(ResourcePackAuthorBlock), new PropertyMetadata(default(ResourcePack)));

        public ResourcePack ResourcePack
        {
            get => GetValue(ResourcePackProperty) as ResourcePack;
            set
            {
                SetValue(ResourcePackProperty, value);

                IsUserAuthor = AuthService.CurrentUser == null
                    ? IsUserAuthor = false
                    : value.Author.Equals(AuthService.CurrentUser.Key);
            }
        }

        private bool IsUserAuthor
        {
            get => EditButton.Visibility == Visibility.Visible; 
            set => EditButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private RelayCommand _editCommand;
        public ICommand EditCommand => _editCommand = _editCommand ?? new RelayCommand(x => Edit());

        private RelayCommand _downloadCommand;
        public ICommand DownloadCommand => _downloadCommand = _downloadCommand ?? new RelayCommand(async x => await Download());

        private async Task Download()
        {
            await ResourcePackService.DownloadToFile(ResourcePack, DownloadProgressChanged);
        }

        private async void DownloadProgressChanged(DownloadOperation sender)
        {
            DownloadProgressBar.Visibility = Visibility.Visible;

            if (sender.Progress.Status == BackgroundTransferStatus.Error)
            {
                DownloadProgressBar.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (sender.Progress.Status == BackgroundTransferStatus.Idle)
            {
                DownloadProgressBar.Foreground = new SolidColorBrush(Colors.Orange);
            }
            else
            {
                DownloadProgressBar.Foreground = new SolidColorBrush(Colors.Green);
            }

            DownloadProgressBar.Minimum = 0;
            DownloadProgressBar.Maximum = sender.Progress.TotalBytesToReceive;
            DownloadProgressBar.Value = sender.Progress.BytesReceived;

            if (sender.Progress.TotalBytesToReceive == sender.Progress.BytesReceived)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                DownloadProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        public ResourcePackAuthorBlock()
        {
            this.InitializeComponent();
        }

        public void Edit()
        {
             NavigationService.Navigate(typeof(ResourcePackSettingsPage), ResourcePack, $"{ResourcePack.Name} Settings");
        }

        private async void UserPictureItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is UserPictureItem userPictureItem))
            {
                return;
            }

            userPictureItem.User = await RestApiService<User>.Get(ResourcePack.Author);
        }
    }
}
