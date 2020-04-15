using ResourceCraft.BackEnd.MinecraftUtilities;
using ResourceCraft.Utilities;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ResourceCraft.Uwp.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel { get; set; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void PageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeApplication();
        }

        private async Task InitializeApplication()
        {
            NavigationService.Lock();

            // Sign in to database
            await AuthService.SignInAsync();
            // Ask user for permission
            await ResourcePackService.EnsureAccessToMinecraftDirectoryAsync();
            await MinecraftService.InitializeAsync();

            AppSettings.PrintSettings();

            NavigationService.Unlock();
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.Initialize(_contentFrame, _navigationView, _progressRing);
            NavigationService.Navigate(typeof(HomePage), null, "Home");

            if (_navigationView.IsPaneOpen)
            {
                NavigationViewItemSearchIcon.Visibility = Visibility.Collapsed;
                NavigationViewItemSearchBox.Visibility = Visibility.Visible;
            }
            else
            {
                NavigationViewItemSearchBox.Visibility = Visibility.Collapsed;
                NavigationViewItemSearchIcon.Visibility = Visibility.Visible;
            }
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private async void NavigationView_Invoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (!(args.InvokedItem is string viewItemContent))
            {
                Logger.WriteLine("Invoked navigation item has no content. No navigation can be done.");
                return;
            }

            if (viewItemContent == "SearchIcon")
            {
                _navigationView.IsPaneOpen = true;
                SearchBox.Focus(FocusState.Keyboard);
            }

            await NavigationService.Navigate(viewItemContent);
        }

        private async void ASB_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                await ViewModel.Search(sender.Text.ToLower());
        }

        private void ASB_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.IsSuggestionListOpen = false;
            ViewModel.SubmitSearch(args);
        }

        private void NavigationView_PaneOpening(NavigationView sender, object args)
        {
            NavigationViewItemSearchIcon.Visibility = Visibility.Collapsed;
            NavigationViewItemSearchBox.Visibility = Visibility.Visible;
        }

        private void NavigationView_PaneClosing(NavigationView sender, object args)
        {
            NavigationViewItemSearchBox.Visibility = Visibility.Collapsed;
            NavigationViewItemSearchIcon.Visibility = Visibility.Visible;
        }
    }
}
