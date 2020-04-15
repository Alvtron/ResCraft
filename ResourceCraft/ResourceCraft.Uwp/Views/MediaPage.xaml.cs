using ResourceCraft.DataAccess;
using ResourceCraft.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Views
{
    public sealed partial class MediaPage : Page
    {
        private Video Video { get; set; }
        private FirebaseFile File { get; set; }

        public MediaPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            switch (e.Parameter)
            {
                case FirebaseFile file:
                    File = file;
                    break;
                case Video video:
                    Video = video;
                    break;
                default:
                    Frame.GoBack();
                    break;
            }
        }

        private async void ImageView_Loaded(object sender, RoutedEventArgs e)
        {
            if (File == null)
            {
                ImageView.Visibility = Visibility.Collapsed;
                return;
            }

            ImageView.Visibility = Visibility.Visible;
            var url = await StorageSource.GetFileUrlAsync($"images/{File.FileName}");
            ImageView.Source = new BitmapImage(url);
        }

        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Video == null || Video.Empty)
            {
                VideoView.Visibility = Visibility.Collapsed;
                return;
            }

            VideoView.Visibility = Visibility.Visible;
            VideoView.Source = Video.YouTubeUri;
        }
    }
}
