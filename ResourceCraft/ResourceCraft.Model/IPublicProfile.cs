using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ResourceCraft.Model
{
    public interface IPublicProfile : IObservableObject
    {
        string Name { get; set; }
        ObservableCollection<ProfilePicture> ProfilePictures { get; set; }
        ObservableCollection<Banner> Banners { get; set; }
        ProfilePicture ProfilePicture { get; }
        Banner Banner { get; }
        void SetProfilePicture(User user, ProfilePicture profilePicture);
        void SetBanner(User user, Banner banner);
    }
}