using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class ResourceSearchView : UserControl
    {
        public static readonly DependencyProperty ResourcePacksProperty = DependencyProperty.Register("ResourcePacks", typeof(IEnumerable<ResourcePack>), typeof(ResourceSearchView), new PropertyMetadata(new List<ResourcePack>()));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(ResourceSearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty SearchQueryProperty = DependencyProperty.Register("SearchQuery", typeof(string), typeof(ResourceSearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty IsSearchBoxEnabledProperty = DependencyProperty.Register("IsSearchBoxEnabled", typeof(bool), typeof(ResourceSearchView), new PropertyMetadata(true));

        private ObservableCollection<ResourcePack> FilteredResourcePacks { get; set; } = new ObservableCollection<ResourcePack>();

        public IEnumerable<ResourcePack> ResourcePacks
        {
            get => GetValue(ResourcePacksProperty) as IEnumerable<ResourcePack>;
            set
            {
                SetValue(ResourcePacksProperty, value);
                SearchByQuery(SearchQuery);
            }
        }
        public string Header
        {
            get => GetValue(HeaderProperty) as string;
            set => SetValue(HeaderProperty, value);
        }

        public string SearchQuery
        {
            get => GetValue(SearchQueryProperty) as string;
            set
            {
                SearchByQuery(value);
                SetValue(SearchQueryProperty, value);
            }
        }

        public bool IsSearchBoxEnabled
        {
            get => (bool)GetValue(IsSearchBoxEnabledProperty);
            set => SetValue(IsSearchBoxEnabledProperty, value);
        }

        public ResourceSearchView()
        {
            InitializeComponent();
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(e.ClickedItem is ResourcePack resourcePack))
            {
                await NotificationService.DisplayErrorMessage("There seems to be something wrong with that resource pack. Sorry about that.");
                return;
            }

            NavigationService.Navigate(typeof(ResourcePackPage), resourcePack, $"{resourcePack.Name}");
        }

        private void UpdateHeader()
        {
            Header = FilteredResourcePacks.Count == 1
                ? $"{FilteredResourcePacks.Count} result"
                : $"{FilteredResourcePacks.Count} results";
        }

        private void UpdateListView(IEnumerable<ResourcePack> codes)
        {
            FilteredResourcePacks.Clear();
            foreach (var code in codes)
            {
                FilteredResourcePacks.Add(code);
            }
            UpdateHeader();
        }

        private void SearchByQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                UpdateListView(ResourcePacks);
                return;
            }

            query = query.ToLower();

            UpdateListView(ResourcePacks?.Where(u => u.Name.ToLower().Contains(query)));
        }
    }
}
