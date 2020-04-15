using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ResourceCraft.Uwp.Dialogs
{
    public sealed partial class AddVideoDialog : ContentDialog
    {
        public Video Video { get; private set; } = new Video();

        public AddVideoDialog()
        {
            if (AuthService.CurrentUser == null)
            {
                Hide();
                return;
            }

            InitializeComponent();
        }

        private void ViewVideoButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Visibility = Visibility.Visible;
            VideoPlayer.Source = Video.YouTubeUri;
        }
    }
}
