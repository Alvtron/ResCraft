using ResourceCraft.BackEnd.MinecraftUtilities;
using ResourceCraft.Minecraft;
using ResourceCraft.Uwp.Services;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DownloadVersionsPage : Page
    {
        public List<string> Versions => MinecraftAPI.Ids;

        public DownloadVersionsPage()
        {
            InitializeComponent();
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.LockFrame();

            var option = (ExtractFiles.IsChecked.HasValue && ExtractFiles.IsChecked.Value)
                ? MinecraftService.DownloadOption.ExtractFiles : MinecraftService.DownloadOption.Default;
            if (DownloadAllCheckbox.IsChecked.HasValue && DownloadAllCheckbox.IsChecked.Value)
                await MinecraftService.DownloadAllAsync(option);
            else
                await MinecraftService.DownloadAsync(VersionBox.SelectedItem as string, option);

            NavigationService.UnlockFrame();
        }
    }
}
