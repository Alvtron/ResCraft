using Newtonsoft.Json;
using ResourceCraft.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ResourceCraft.Model
{
    public class ResourcePack : Content, IPublicProfile
    {
        public FirebaseKey Author { get; set; }

        private string _version;
        public string Version
        {
            get => _version;
            set => SetField(ref _version, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        private string _about;
        public string About
        {
            get => _about;
            set => SetField(ref _about, value);
        }

        private int _downloads;
        public int Downloads
        {
            get => _downloads;
            set => SetField(ref _downloads, value);
        }

        public ObservableCollection<ResourceFile> Resources { get; set; } = new ObservableCollection<ResourceFile>();
        public ObservableCollection<Log> Logs { get; set; } = new ObservableCollection<Log>();
        public ObservableCollection<User> Collaborators { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<Post> Updates { get; set; } = new ObservableCollection<Post>();

        [JsonIgnore, NotMapped]
        public string FileName => $"{Key.Uid}.zip";

        // CONSTRUCTORS

        public ResourcePack() { }

        public ResourcePack(User author, string name, string version, string description = null)
        {
            Author = author.Key;
            Name = name;
            Version = version;
            Description = description;
        }

        #region Methods

        public void AddResource(User user, ResourceFile resource)
        {
            if (resource == null)
            {
                Logger.WriteLine("Can't add null resource. Nothing happens.");
                return;
            }
            if (Updates == null)
            {
                Resources = new ObservableCollection<ResourceFile>();
            }
            Resources.Add(resource);
            user.IncreaseExperience(Experience.Action.AddResource);
            Logs.Add(new Log(user.Key, "added", resource.Key));
        }

        public void AddUpdate(User user, Post post)
        {
            if (post == null)
            {
                Logger.WriteLine("Can't add null post. Nothing happens.");
                return;
            }
            if (Updates == null)
            {
                Updates = new ObservableCollection<Post>();
            }
            Updates.Add(post);
            user.IncreaseExperience(Experience.Action.AddPost);
            Logs.Add(new Log(user.Key, "added", post.Key));
        }

        public override string ToString()
        {
            return Name;
        }

        public override void SetProfilePicture(User user, ProfilePicture profilePicture)
        {
            SetProfilePicture(profilePicture);
            user.IncreaseExperience(Experience.Action.UploadImage);
            Logs.Add(new Log(user.Key, "uploaded", profilePicture.Key));
        }

        public override void SetBanner(User user, Banner banner)
        {
            SetBanner(banner);
            user.IncreaseExperience(Experience.Action.UploadImage);
            Logs.Add(new Log(user.Key, "uploaded", banner.Key));
        }

        public override void AddScreenshot(User user, Screenshot screenshot)
        {
            AddScreenshot(screenshot);
            user.IncreaseExperience(Experience.Action.UploadImage);
            Logs.Add(new Log(user.Key, "added", screenshot.Key));
        }

        public override void AddVideo(User user, Video video)
        {
            AddVideo(video);
            user.IncreaseExperience(Experience.Action.UploadVideo);
            Logs.Add(new Log(user.Key, "added", video.Key));
        }

        public override void Reply(User user, Reply comment)
        {
            Reply(comment);
            user.IncreaseExperience(Experience.Action.AddReply);
        }

        #endregion
    }
}
