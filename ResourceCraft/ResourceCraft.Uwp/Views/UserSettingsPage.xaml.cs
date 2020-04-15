using ResourceCraft.Model;
using ResourceCraft.DataAccess;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.ViewModels;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Views
{
    public sealed partial class UserSettingsPage : Page
    {
        private UserSettingsViewModel ViewModel { get; set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.Lock();

            User user;

            switch (e.Parameter)
            {
                case string uid:
                    user = await RestApiService<User>.Get(new FirebaseKey(uid, typeof(User).Name));
                    break;
                case FirebaseKey key:
                    user = await RestApiService<User>.Get(key);
                    break;
                case IFirebaseEntity databaseItem:
                    user = await RestApiService<User>.Get(databaseItem.Key);
                    break;
                default:
                    await NotificationService.DisplayErrorMessage("Developer error.");
                    throw new InvalidOperationException();
            }

            if (user == null)
            {
                await NotificationService.DisplayErrorMessage("This user does not exist.");
                NavigationService.GoBack();
            }

            ViewModel = new UserSettingsViewModel(user);

            InitializeComponent();

            NavigationService.Unlock();

            NavigationService.SetHeaderTitle($"{ViewModel.Model?.UserName} - Settings");
        }
    }
}
