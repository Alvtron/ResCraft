using ResourceCraft.DataAccess;
using ResourceCraft.Model;
using ResourceCraft.Utilities;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using ResourceCraft.Uwp.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace ResourceCraft.Uwp.ViewModels
{
    public class AddResourcePackViewModel : DialogViewModel
    {
        private bool _changed;
        public bool Changed
        {
            get => _changed;
            set => SetField(ref _changed, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        private StorageFile _file;
        public StorageFile File
        {
            get => _file;
            set => SetField(ref _file, value);
        }

        private RelayCommand _addResourcePackFromFileCommand;
        public ICommand AddResourcePackFromFileCommand => _addResourcePackFromFileCommand = _addResourcePackFromFileCommand ?? new RelayCommand(async param => await AddResourcePackFromFileAsync());

        public AddResourcePackViewModel()
        {
            if (AuthService.CurrentUser == null)
            {
                NavigationService.GoBack();
                return;
            }
        }

        private async Task AddResourcePackFromFileAsync()
        {
            File = await StorageUtilities.PickSingleFile();
        }

        public async Task<bool> UploadResourcePackAsync()
        {
            if (File == null)
            {
                Logger.WriteLine("File not found.");
                return false;
            }

            NavigationService.Lock();

            var resourcePack = await ResourcePackService.CreateNewResourcePackAsync(File, AuthService.CurrentUser, Name, Description);

            NavigationService.Unlock();

            if (resourcePack == null)
            {
                Logger.WriteLine("Upload failed.");
            }

            NavigationService.Navigate(typeof(ResourcePackPage), resourcePack, resourcePack.Name);
            return true;
        }
    }
}
