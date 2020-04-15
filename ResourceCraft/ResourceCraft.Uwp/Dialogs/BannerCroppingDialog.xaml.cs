using ResourceCraft.Model;
using ResourceCraft.Uwp.Utilities;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Dialogs
{
    public sealed partial class BannerCroppingDialog : ContentDialog
    {
        private ImageCropper<Banner> ImageCropper { get; }

        public Banner Result => ImageCropper.Image;

        public BannerCroppingDialog(Banner banner)
        {
            ImageCropper = new ImageCropper<Banner>(banner);
            InitializeComponent();
        }
    }
}
