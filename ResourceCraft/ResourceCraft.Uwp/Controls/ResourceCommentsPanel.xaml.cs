using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using ResourceCraft.Uwp.Controls;
using ResourceCraft.Uwp.Utilities;
using ResourceCraft.Uwp.Services;
using ResourceCraft.DataAccess;
using ResourceCraft.Model;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class ResourcePackCommentsPanel : UserControl
    {
        public static readonly DependencyProperty ResourcePackProperty = DependencyProperty.Register("ResourcePack", typeof(ResourcePack), typeof(ResourcePackCommentsPanel), new PropertyMetadata(default(ResourcePack)));

        private RelayCommand<Editor> _uploadCommand;
        public ICommand UploadCommand => _uploadCommand = _uploadCommand ?? new RelayCommand<Editor>(async editor => await UploadCommentAsync(editor));

        public ResourcePack ResourcePack
        {
            get => GetValue(ResourcePackProperty) as ResourcePack;
            set => SetValue(ResourcePackProperty, value);
        }

        public ResourcePackCommentsPanel()
        {
            InitializeComponent();
        }

        public async Task UploadCommentAsync(Editor editor)
        {
            if (AuthService.CurrentUser == null)
            {
                await NotificationService.DisplayErrorMessage("You are not logged in!");
                return;
            }

            if (editor == null || string.IsNullOrWhiteSpace(editor.Rtf))
            {
                await NotificationService.DisplayErrorMessage("Can't post empty comment!");
                return;
            }

            NavigationService.Lock();

            var comment = new Reply(ResourcePack.Key, AuthService.CurrentUser, editor.Rtf);

            ResourcePack.Reply(AuthService.CurrentUser, comment);

            if (!await RestApiService<ResourcePack>.Update(ResourcePack))
            {
                await NotificationService.DisplayErrorMessage("Something went wrong when uploading your comment.");
            }
            else
            {
                editor.Clear();
            }

            NavigationService.Unlock();
        }
    }
}
