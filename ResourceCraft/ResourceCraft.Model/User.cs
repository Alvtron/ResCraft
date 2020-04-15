using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ResourceCraft.Utilities;
using System.Collections;

namespace ResourceCraft.Model
{
    public class User : FirebaseEntity, IPublicProfile
    {
        private string _userName;
        public string UserName
        {
            get => _userName;
            set => SetField(ref _userName, value);
        }
        public string Name { get => UserName; set => UserName = value; }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => SetField(ref _firstName, value);
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => SetField(ref _lastName, value);
        }

        private DateTime? _birthday;
        public DateTime? Birthday
        {
            get => _birthday;
            set => SetField(ref _birthday, value);
        }

        private string _gender;
        public string Gender
        {
            get => _gender;
            set => SetField(ref _gender, value);
        }

        private string _country;
        public string Country
        {
            get => _country;
            set => SetField(ref _country, value);
        }

        private string _bio;
        public string Bio
        {
            get => _bio;
            set => SetField(ref _bio, value);
        }

        private bool _subscribed;
        public bool Subscribed
        {
            get => _subscribed;
            set => SetField(ref _subscribed, value);
        }

        private string _website;
        public string Website
        {
            get => _website;
            set => SetField(ref _website, value);
        }
        [NotMapped, JsonIgnore]
        public Uri WebsiteUri => Uri.TryCreate(Website, UriKind.Absolute, out Uri uri) ? uri : null;

        [Required]
        private DateTime? _signedIn = DateTime.Now;
        public DateTime? SignedIn
        {
            get => _signedIn;
            set => SetField(ref _signedIn, value);
        }

        private DateTime? _signedOut;
        public DateTime? SignedOut
        {
            get => _signedOut;
            set => SetField(ref _signedOut, value);
        }

        private Color _color = new Color();
        public Color Color
        {
            get => _color;
            set => SetField(ref _color, value);
        }

        private int _experience;
        public int Experience
        {
            get => _experience;
            set => SetField(ref _experience, value);
        }

        private int _views;
        public int Views
        {
            get => _views;
            set => SetField(ref _views, value);
        }

        public ObservableCollection<User> Friends { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<ProfilePicture> ProfilePictures { get; set; } = new ObservableCollection<ProfilePicture>();
        public ObservableCollection<Banner> Banners { get; set; } = new ObservableCollection<Banner>();
        public ObservableCollection<Log> Logs { get; set; } = new ObservableCollection<Log>();
        public ObservableCollection<FirebaseKey> ResourcePacks { get; set; } = new ObservableCollection<FirebaseKey>();

        [NotMapped, JsonIgnore]
        public Banner Banner => Banners.FirstOrDefault(p => p.IsPrimary);
        [NotMapped, JsonIgnore]
        public ProfilePicture ProfilePicture => ProfilePictures.FirstOrDefault(p => p.IsPrimary);
        [NotMapped, JsonIgnore]
        public bool HasBanners => Banners.Count > 0;

        // CONSTRUCTORS

        public User()
        {
        }

        // FUNCTIONS

        public int IncreaseExperience(Experience.Action action) => Experience += (int)action;

        public int DecreaseExperience(int amount) => Experience -= amount < 0 ? -1 : amount;

        public void SignIn()
        {
            SignedIn = DateTime.Now;
            IncreaseExperience(Model.Experience.Action.SignIn);
            Logs.Add(new Log(Key, "signed in", null, false));
        }

        public void SignOut()
        {
            SignedOut = DateTime.Now;
            Logs.Add(new Log(Key, "signed out", null, false));
        }

        public void SetProfilePicture(User user, ProfilePicture profilePicture)
        {
            if (profilePicture == null)
            {
                throw new InvalidOperationException("Profile Picture is null.");
            }

            var existingProfilePicture = ProfilePictures.FirstOrDefault(i => i.Key.Equals(profilePicture.Key));
            if (existingProfilePicture == null)
            {
                ProfilePictures.Add(profilePicture);
            }
            else
            {
                ProfilePictures.Remove(existingProfilePicture);
                ProfilePictures.Add(profilePicture);
            }

            for (var i = 0; i < ProfilePictures.Count; i++)
            {
                ProfilePictures[i].IsPrimary = profilePicture.Key.Equals(ProfilePictures[i].Key);
            }

            IncreaseExperience(Model.Experience.Action.UploadImage);
            Logs.Add(new Log(user.Key, "uploaded", profilePicture.Key));
            RefreshBindings();
        }

        public void SetBanner(User user, Banner banner)
        {
            if (banner == null)
                throw new NullReferenceException("Banner was null.");
            if (Banners == null)
                Banners = new ObservableCollection<Banner>();

            var existingBanner = Banners.FirstOrDefault(i => i.Key.Equals(banner.Key));
            if (existingBanner == null)
            {
                Banners.Add(banner);
            }
            else
            {
                Banners.Remove(existingBanner);
                Banners.Add(banner);
            }

            for (var i = 0; i < Banners.Count; i++)
            {
                Banners[i].IsPrimary = banner.Key.Equals(Banners[i].Key);
            }

            RefreshBindings();
            IncreaseExperience(Model.Experience.Action.UploadImage);
            Logs.Add(new Log(user.Key, "uploaded", banner.Key));
        }

        public void AddFriend(User friend)
        {
            if (friend == null)
            {
                Logger.WriteLine($"Can't befriend {UserName} with an uninitialized user object.");
                return;
            }
            if (Key == friend.Key)
            {
                Logger.WriteLine($"Can't befriend {UserName} and {friend.UserName} as they are the same user.");
                return;
            }
            if (IsFriendsWith(friend))
            {
                Logger.WriteLine($"Can't befriend {UserName} and {friend.UserName} as they are already friends.");
                return;
            }

            Friends.Add(friend);
            IncreaseExperience(Model.Experience.Action.Befriend);
            Logs.Add(new Log(Key, "is now friends with", friend.Key));
        }

        public void RemoveFriend(User friend)
        {
            if (friend == null)
            {
                Logger.WriteLine($"Can't unfriend {UserName} with an uninitialized user object.");
                return;
            }
            if (Key == friend.Key)
            {
                Logger.WriteLine($"Can't unfriend {UserName} and {friend.UserName} as they are the same user.");
                return;
            }
            if (!IsFriendsWith(friend))
            {
                Logger.WriteLine($"Can't unfriend {UserName} and {friend.UserName} as they are not friends.");
                return;
            }

            Friends.Remove(friend);
            Logs.Add(new Log(Key, "is no longer friends with", friend.Key));
        }

        public void SetColor(byte r, byte g, byte b, byte a)
        {
            SetColor(new Color(r, g, b, a));
        }

        public void SetColor(Color color)
        {
            if (color == null) return;

            Color = color;
            IncreaseExperience(Model.Experience.Action.ChangedSettings);
            Logs.Add(new Log(Key, $"changed his color to {color}"));
        }

        public bool IsFriendsWith(User friend) => Friends.Any(f => f.Key == friend.Key);

        // NOT MAPPED

        [NotMapped, JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped, JsonIgnore]
        public string Initials => (!string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName)) ? FirstName?.Substring(0, 1) + LastName?.Substring(0, 1) : "";

        [NotMapped, JsonIgnore]
        public int CurrentLevel => Model.Experience.ExpToLevel(Experience);

        [NotMapped, JsonIgnore]
        public int NextLevel => CurrentLevel + 1;

        [NotMapped, JsonIgnore]
        public int NextExp => NextLevel * Model.Experience.LevelUpExp;

        // OVERRIDES

        public override string ToString() => UserName;
    }
}