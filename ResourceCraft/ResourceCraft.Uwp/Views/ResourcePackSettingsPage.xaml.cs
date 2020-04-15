using ResourceCraft.DataAccess;
using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using ResourceCraft.Uwp.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ResourceCraft.Uwp.Views
{
    public sealed partial class ResourcePackSettingsPage : Page
    {
        private ResourcePackSettingsViewModel ViewModel;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.Lock();

            ResourcePack resourcePack;

            switch (e.Parameter)
            {
                case string uid:
                    resourcePack = await RestApiService<ResourcePack>.Get(new FirebaseKey(uid, typeof(ResourcePack).Name));
                    break;
                case FirebaseKey key:
                    resourcePack = await RestApiService<ResourcePack>.Get(key);
                    break;
                case IFirebaseEntity databaseItem:
                    resourcePack = await RestApiService<ResourcePack>.Get(databaseItem.Key);
                    break;
                default:
                    await NotificationService.DisplayErrorMessage("Developer error.");
                    throw new InvalidOperationException();
            }

            if (resourcePack == null)
            {
                await NotificationService.DisplayErrorMessage("This code does not exist.");
                NavigationService.GoBack();
            }

            ViewModel = new ResourcePackSettingsViewModel(resourcePack);

            InitializeComponent();

            NavigationService.Unlock();

            NavigationService.SetHeaderTitle($"{ViewModel.Model?.Name} - Settings");
        }
    }
}
