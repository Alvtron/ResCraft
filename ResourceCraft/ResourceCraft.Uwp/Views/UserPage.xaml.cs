using ResourceCraft.Model;
using ResourceCraft.DataAccess;
using ResourceCraft.Utilities;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.ViewModels;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ResourceCraft.Uwp.Views
{
    public sealed partial class UserPage : Page
    {
        private UserViewModel ViewModel { get; set; }

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

            ViewModel = new UserViewModel(user);

            InitializeComponent();

            NavigationService.Unlock();

            ViewModel.Model.Views++;

            if (!await RestApiService<User>.Update(ViewModel.Model))
            {
                Logger.WriteLine($"Failed to increment view counter for user {ViewModel.Model.Key}.");
            }

            NavigationService.SetHeaderTitle($"{ViewModel.Model?.UserName}'s profile");
        }
    }
}