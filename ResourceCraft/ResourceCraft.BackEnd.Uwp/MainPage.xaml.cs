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
using ResourceCraft.Minecraft;
using ResourceCraft.BackEnd.MinecraftUtilities;
using ResourceCraft.DataAccess;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ResourceCraft.BackEnd.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void LockView()
        {
            AppProgressRing.IsActive = true;
            IsEnabled = false;
        }

        private void UnlockView()
        {
            AppProgressRing.IsActive = false;
            IsEnabled = true;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await AuthDataSource.SignIn("thomas.angeland@gmail.com", "#Alvtron1");
        }

        private async void DownloadMinecraftVersionsButton_Click(object sender, RoutedEventArgs e)
        {
            LockView();
            await MinecraftService.InitializeAsync();
            UnlockView();
        }

        private async void DownloadMinecraftResourcesButton_Click(object sender, RoutedEventArgs e)
        {
            LockView();
            await MinecraftService.DownloadAllAsync();
            UnlockView();
        }

        private async void CreateMinecraftRelationsButton_Click(object sender, RoutedEventArgs e)
        {
            LockView();
            await ResourceRelationService.CreateAsync();
            UnlockView();
        }

        private async void LoadMinecraftRelationsButton_Click(object sender, RoutedEventArgs e)
        {
            LockView();
            await ResourceRelationService.LoadAsync();
            UnlockView();
        }
    }
}
