using System;
using ResourceCraft.Uwp.Dialogs;
using ResourceCraft.Uwp.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Views
{
    public sealed partial class HomePage : Page
    {
        private HomeViewModel ViewModel { get; set; } = new HomeViewModel();

        public HomePage()
        {
            InitializeComponent();
        }

        private async void NewUsersGridView_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.RefreshNewestUsers();
        }
    }
}
