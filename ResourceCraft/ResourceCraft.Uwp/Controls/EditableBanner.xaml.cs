using ResourceCraft.Model;
using ResourceCraft.Uwp.Dialogs;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class EditableBanner : UserControl
    {
        public static readonly DependencyProperty PublicProfileProperty = DependencyProperty.Register("PublicProfile", typeof(IPublicProfile), typeof(EditableBanner), new PropertyMetadata(default(IPublicProfile)));
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(int), typeof(EditableBanner), new PropertyMetadata(100));
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(EditableBanner), new PropertyMetadata(false));
        private static new readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(int), typeof(EditableBanner), new PropertyMetadata(800));
        private static new readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(int), typeof(EditableBanner), new PropertyMetadata(320));

        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        public new int Width
        {
            get => (int)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public new int Height
        {
            get => (int)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public int Size
        {
            get => (int)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public IPublicProfile PublicProfile
        {
            get => GetValue(PublicProfileProperty) as IPublicProfile;
            set => SetValue(PublicProfileProperty, value);
        }

        private RelayCommand _uploadImageCommand;
        public ICommand UploadImageCommand => _uploadImageCommand = _uploadImageCommand ?? new RelayCommand(async param => await UploadImageAsync());

        private RelayCommand _cropImageCommand;
        public ICommand CropImageCommand => _cropImageCommand = _cropImageCommand ?? new RelayCommand(async param => await CropImageAsync());

        private RelayCommand _editImagesCommand;
        public ICommand EditImagesCommand => _editImagesCommand = _editImagesCommand ?? new RelayCommand(async param => await EditImagesAsync());

        public EditableBanner()
        {
            InitializeComponent();
        }

        public async Task UploadImageAsync()
        {
            var dialog = new UploadImageDialog();
            await dialog.ShowAsync();
            var image = await dialog.CreateImageFromFile<Banner>();

            if (image == null)
            {
                return;
            }

            PublicProfile.SetBanner(AuthService.CurrentUser, image);
        }

        public async Task CropImageAsync()
        {
            if (PublicProfile?.Banner == null)
            {
                return;
            }

            var dialog = new BannerCroppingDialog(PublicProfile.Banner);

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                return;
            }

            PublicProfile.RefreshBindings();
        }

        public async Task EditImagesAsync()
        {
            if (PublicProfile?.Banners?.Count == 0)
            {
                return;
            }

            var dialog = new BannerManagerDialog(PublicProfile.Banners, $"{PublicProfile.Name} - Profile Banners");
            await dialog.ShowAsync();

            PublicProfile.RefreshBindings();
        }
    }
}
