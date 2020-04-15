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
    public sealed partial class EditableProfilePicture : UserControl
    {
        public static readonly DependencyProperty PublicProfileProperty = DependencyProperty.Register("PublicProfile", typeof(IPublicProfile), typeof(EditableProfilePicture), new PropertyMetadata(default(IPublicProfile)));
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(int), typeof(EditableProfilePicture), new PropertyMetadata(100));
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(EditableProfilePicture), new PropertyMetadata(false));
        private static new readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(int), typeof(EditableProfilePicture), new PropertyMetadata(100));
        private static new readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(int), typeof(EditableProfilePicture), new PropertyMetadata(100));

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

        public EditableProfilePicture()
        {
            InitializeComponent();
        }

        public async Task UploadImageAsync()
        {
            var dialog = new UploadImageDialog();
            await dialog.ShowAsync();
            var image = await dialog.CreateImageFromFile<ProfilePicture>();

            if (image == null)
            {
                return;
            }

            PublicProfile.SetProfilePicture(AuthService.CurrentUser, image);
        }

        public async Task CropImageAsync()
        {
            if (PublicProfile?.ProfilePicture == null)
            {
                return;
            }

            var dialog = new ProfilePictureCroppingDialog(PublicProfile.ProfilePicture);

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                return;
            }

            PublicProfile.RefreshBindings();
        }

        public async Task EditImagesAsync()
        {
            if (PublicProfile?.ProfilePictures?.Count == 0)
            {
                return;
            }

            var dialog = new ProfilePicturesManagerDialog(PublicProfile.ProfilePictures, $"{PublicProfile.Name} - Profile Pictures");
            await dialog.ShowAsync();

            PublicProfile.RefreshBindings();
        }
    }
}
