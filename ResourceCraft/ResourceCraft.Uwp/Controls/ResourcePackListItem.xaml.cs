using ResourceCraft.DataAccess;
using ResourceCraft.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class ResourcePackListItem : UserControl
    {
        public static readonly DependencyProperty ResourcePackProperty = DependencyProperty.Register("ResourcePack", typeof(ResourcePack), typeof(ResourcePackListItem), new PropertyMetadata(new ResourcePack()));

        public ResourcePack ResourcePack
        {
            get => GetValue(ResourcePackProperty) as ResourcePack;
            set
            {
                SetValue(ResourcePackProperty, value);
                DownloadUserAsync();
            }
        }

        public ResourcePackListItem()
        {
            this.InitializeComponent();
        }

        private async void DownloadUserAsync()
        {
            var user = await RestApiService<User>.Get(ResourcePack?.Author);
            CaptionBlock.Text = user == null
                ? $"Anonymous • {ResourcePack.Views} views"
                : $"{user.UserName} • {ResourcePack.Views} views";
        }
    }
}
