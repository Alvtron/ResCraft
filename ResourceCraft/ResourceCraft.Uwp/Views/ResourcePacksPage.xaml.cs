using ResourceCraft.DataAccess;
using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResourcePacksPage : Page
    {
        private ResourcePacksViewModel ViewModel;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.Lock();

            var resourcePacks = await RestApiService<ResourcePack>.Get();

            if (resourcePacks == null)
            {
                await NotificationService.DisplayErrorMessage("Could not retrieve resource packs from database.");
                NavigationService.GoBack();
            }

            ViewModel = new ResourcePacksViewModel(resourcePacks);

            InitializeComponent();

            NavigationService.Unlock();

            NavigationService.SetHeaderTitle("Resource Packs");
        }
    }
}
