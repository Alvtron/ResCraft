using ResourceCraft.Model;
using ResourceCraft.DataAccess;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ResourceCraft.Utilities;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Views
{
    public sealed partial class ResourcePackPage : Page
    {
        private ResourcePackViewModel ViewModel { get; set; }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.Lock();

            ResourcePack resourcePack;

            switch (e.Parameter)
            {
                case FirebaseKey key:
                    resourcePack = await RestApiService<ResourcePack>.Get(key);
                    break;
                case IFirebaseEntity entity:
                    resourcePack = await RestApiService<ResourcePack>.Get(entity.Key);
                    break;
                default:
                    await NotificationService.DisplayErrorMessage("Developer error.");
                    throw new InvalidOperationException();
            }

            if (resourcePack == null)
            {
                await NotificationService.DisplayErrorMessage("This resource pack does not exist.");
                NavigationService.GoBack();
            }

            ViewModel = new ResourcePackViewModel(resourcePack);

            InitializeComponent();

            NavigationService.Unlock();

            ViewModel.Model.Views++;

            if (!await RestApiService<ResourcePack>.Update(ViewModel.Model))
            {
                Logger.WriteLine($"Failed to increment view counter for code {ViewModel.Model.Key}.");
            }

            NavigationService.SetHeaderTitle(ViewModel.Model?.Name);
        }
    }
}
