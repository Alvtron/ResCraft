using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Views;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class ResourcePackGridView : UserControl
    {
        public static readonly DependencyProperty ResourcePacksProperty = DependencyProperty.Register("ResourcePacks", typeof(IEnumerable<ResourcePack>), typeof(ResourcePackGridView), new PropertyMetadata(new List<ResourcePack>()));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(ResourcePackGridView), new PropertyMetadata(null));

        public IEnumerable<ResourcePack> ResourcePacks
        {
            get => GetValue(ResourcePacksProperty) as IEnumerable<ResourcePack>;
            set => SetValue(ResourcePacksProperty, value);
        }
        public string Header
        {
            get => GetValue(HeaderProperty) as string;
            set => SetValue(HeaderProperty, value);
        }

        public ResourcePackGridView()
        {
            this.InitializeComponent();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ResourcePack code)
            {
                NavigationService.Navigate(typeof(ResourcePackPage), code.Key, code.Name);
            }
        }
    }
}
