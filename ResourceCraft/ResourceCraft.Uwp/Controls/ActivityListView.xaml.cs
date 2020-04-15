using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using ResourceCraft.DataAccess;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class ActivityListView : UserControl
    {
        public static readonly DependencyProperty LogsSourceProperty = DependencyProperty.Register("LogsSource", typeof(object), typeof(ActivityListView), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(ActivityListView), new PropertyMetadata(null));

        public object LogsSource
        {
            get => GetValue(LogsSourceProperty);
            set
            {
                if (value is IEnumerable<ILog> logs)
                {
                    SetValue(LogsSourceProperty, logs.OrderByDescending(l => l.Created));
                }
            }
        }

        public string Header
        {
            get => GetValue(HeaderProperty) as string;
            set => SetValue(HeaderProperty, value);
        }

        public ActivityListView()
        {
            InitializeComponent();
        }

        private async void Actor_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is HyperlinkButton link)) return;
            if (!(link.Tag is ILog log)) return;

            await NavigationService.Navigate(log.Actor?.Group, log.Actor?.Uid);
        }

        private async void Subject_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is HyperlinkButton link)) return;
            if (!(link.Tag is ILog log)) return;

            await NavigationService.Navigate(log.Subject?.Group, log.Subject?.Uid);
        }

        private async void Actor_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is HyperlinkButton link)) return;
            if (!(link.Tag is ILog log)) return;
            if (string.IsNullOrWhiteSpace(log.Actor.Uid)) return;

            await PopulateLinkContent(link, log.Actor?.Group, log.Actor?.Uid);
        }

        private async void Subject_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is HyperlinkButton link)) return;
            if (!(link.Tag is ILog log)) return;
            if (string.IsNullOrWhiteSpace(log.Subject?.Uid)) return;

            await PopulateLinkContent(link, log.Subject?.Group, log.Subject?.Uid);
        }

        private async Task PopulateLinkContent(HyperlinkButton link, string type, string key)
        {
            switch (type)
            {
                case "File":
                    var file = await RestApiService<FirebaseFile>.Get(new FirebaseKey(key, "File"));
                    if (file != null) link.Content = file?.FileName;
                    return;
                case "User":
                    var user = await RestApiService<User>.Get(new FirebaseKey(key, "User"));
                    if (user != null) link.Content = user?.UserName;
                    return;
                default:
                    break;
            }
        }
    }
}
