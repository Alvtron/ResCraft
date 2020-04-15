using ResourceCraft.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCraft.Uwp.ViewModels
{
    class ResourcePacksViewModel : ObservableObject
    {
        private ObservableCollection<ResourcePack> _resourcepacks;
        public ObservableCollection<ResourcePack> ResourcePacks
        {
            get => _resourcepacks;
            set => SetField(ref _resourcepacks, value);
        }

        public ResourcePacksViewModel(IEnumerable<ResourcePack> resourcepacks)
        {
            if (resourcepacks == null)
            {
                throw new ArgumentNullException("Codes was null");
            }

            ResourcePacks = new ObservableCollection<ResourcePack>(resourcepacks);
        }
    }
}
