using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ResourceCraft.Model
{
    public abstract partial class Content : FirebaseEntity, ILikeable
    {
        #region Properties

        private string _name;
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private int _views;
        public int Views
        {
            get => _views;
            set => SetField(ref _views, value);
        }

        private ObservableCollection<ProfilePicture> _profilePictures = new ObservableCollection<ProfilePicture>();
        public ObservableCollection<ProfilePicture> ProfilePictures
        {
            get => _profilePictures;
            set => SetField(ref _profilePictures, value);
        }

        private ObservableCollection<Screenshot> _screenshots = new ObservableCollection<Screenshot>();
        public ObservableCollection<Screenshot> Screenshots
        {
            get => _screenshots;
            set => SetField(ref _screenshots, value);
        }

        private ObservableCollection<Banner> _banners = new ObservableCollection<Banner>();
        public ObservableCollection<Banner> Banners
        {
            get => _banners;
            set => SetField(ref _banners, value);
        }

        private ObservableCollection<Video> _videos = new ObservableCollection<Video>();
        public ObservableCollection<Video> Videos
        {
            get => _videos;
            set => SetField(ref _videos, value);
        }

        private ObservableCollection<Reply> _replies = new ObservableCollection<Reply>();
        public ObservableCollection<Reply> Replies
        {
            get => _replies;
            set => SetField(ref _replies, value);
        }

        private ObservableCollection<Rating> _ratings = new ObservableCollection<Rating>();
        public ObservableCollection<Rating> Ratings
        {
            get => _ratings;
            set => SetField(ref _ratings, value);
        }

        [NotMapped, JsonIgnore]
        public ProfilePicture ProfilePicture => ProfilePictures.FirstOrDefault(p => p.IsPrimary);
        [NotMapped, JsonIgnore]
        public Banner Banner => Banners.FirstOrDefault(p => p.IsPrimary);
        [NotMapped, JsonIgnore]
        public bool HasVideos => Videos != null && Videos.Any();
        [NotMapped, JsonIgnore]
        public bool HasProfilePicture => ProfilePictures != null && ProfilePictures.Any();
        [NotMapped, JsonIgnore]
        public bool HasImages => Screenshots != null && Screenshots.Any();
        [NotMapped, JsonIgnore]
        public bool HasBanners => Banners != null && Banners.Any();
        [NotMapped, JsonIgnore]
        public bool HasReplies => Replies?.Count > 0;
        [NotMapped, JsonIgnore]
        public bool HasRatings => Ratings?.Count > 0;
        [NotMapped, JsonIgnore]
        public int NumberOfReplies => Replies?.Count ?? 0;
        [NotMapped, JsonIgnore]
        public bool HasLikes => Ratings?.Count > 0;
        [NotMapped, JsonIgnore]
        public int NumberOfLikes => Ratings?.Count(x => x.Value == Rating.Type.Like) ?? 0;
        [NotMapped, JsonIgnore]
        public int NumberOfDislikes => Ratings?.Count(x => x.Value == Rating.Type.Dislike) ?? 0;

        public bool HasLiked(User user) => Ratings.Any(i => i.UserKey.Equals(user.Key) && i.Value == Rating.Type.Like);
        public bool HasDisliked(User user) => Ratings.Any(i => i.UserKey.Equals(user.Key) && i.Value == Rating.Type.Dislike);

        #endregion

        #region Methods

        public abstract void SetProfilePicture(User user, ProfilePicture profilePicture);
        protected void SetProfilePicture(ProfilePicture profilePicture)
        {
            if (profilePicture == null)
                throw new NullReferenceException("Thumbnail was null.");
            if (Videos == null)
                Videos = new ObservableCollection<Video>();

            var existingThumbnail = ProfilePictures.FirstOrDefault(i => i.Key.Equals(profilePicture.Key));
            if (existingThumbnail == null)
            {
                ProfilePictures.Add(profilePicture);
            }
            else
            {
                ProfilePictures.Remove(existingThumbnail);
                ProfilePictures.Add(profilePicture);
            }

            for (var i = 0; i < Banners.Count; i++)
            {
                Banners[i].IsPrimary = profilePicture.Key.Equals(Banners[i].Key);
            }

            RefreshBindings();
        }

        public abstract void SetBanner(User user, Banner banner);
        protected void SetBanner(Banner banner)
        {
            if (banner == null)
                throw new NullReferenceException("Video was null.");
            if (Videos == null)
                Videos = new ObservableCollection<Video>();

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
        }

        public abstract void AddScreenshot(User user, Screenshot screenshot);
        protected void AddScreenshot(Screenshot screenshot)
        {
            if (screenshot == null)
                throw new NullReferenceException("Screenshot was null.");
            if (Screenshots == null)
                Screenshots = new ObservableCollection<Screenshot>();

            Screenshots.Add(screenshot);
        }

        public abstract void AddVideo(User user, Video video);
        protected void AddVideo(Video video)
        {
            if (video == null || video.Empty)
                throw new NullReferenceException("Video was null.");
            if (Videos == null)
                Videos = new ObservableCollection<Video>();

            Videos.Add(video);
        }

        public abstract void Reply(User user, Reply comment);
        protected void Reply(Reply comment)
        {
            if (comment == null)
            {
                throw new NullReferenceException("Comment was null.");
            }
            if (Replies == null)
            {
                Replies = new ObservableCollection<Reply>();
            }

            Replies.Add(comment);

            Replies = new ObservableCollection<Reply>(Replies.OrderByDescending(c => c.Created));
        }

        public void Like(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (Ratings == null)
            {
                Ratings = new ObservableCollection<Rating>();
            }

            var rating = Ratings.FirstOrDefault(i => i.UserKey.Equals(user.Key));

            if (rating == null)
            {
                Ratings.Add(new Rating(user, Rating.Type.Like));
            }
            else
            {
                rating.Value = Rating.Type.Like;
            }
        }

        public void Dislike(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (Ratings == null)
            {
                Ratings = new ObservableCollection<Rating>();
            }

            var rating = Ratings.FirstOrDefault(i => i.UserKey.Equals(user.Key));

            if (rating == null)
            {
                Ratings.Add(new Rating(user, Rating.Type.Dislike));
            }
            else
            {
                rating.Value = Rating.Type.Dislike;
            }
        }

        #endregion
    }
}
