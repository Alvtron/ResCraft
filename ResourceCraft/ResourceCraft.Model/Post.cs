using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;

namespace ResourceCraft.Model
{
    public class Post : FirebaseEntity, ILikeable
    {
        public FirebaseKey Author { get; set; }
        private string _title;
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }
        private string _text;
        public string Text
        {
            get => _text;
            set => SetField(ref _text, value);
        }
        public ObservableCollection<Screenshot> Screenshots { get; set; } = new ObservableCollection<Screenshot>();
        public ObservableCollection<Video> Videos { get; set; } = new ObservableCollection<Video>();
        public ObservableCollection<Reply> Replies { get; set; } = new ObservableCollection<Reply>();
        public ObservableCollection<Rating> Ratings { get; set; } = new ObservableCollection<Rating>();

        public Post() { }

        public Post(User user, string title, string text)
        {
            Author = user.Key;
            Title = title;
            Text = text;
        }

        [NotMapped, JsonIgnore]
        public int NumberOfLikes => Ratings?.Count(x => x.Value == Rating.Type.Like) ?? 0;
        [NotMapped, JsonIgnore]
        public int NumberOfDislikes => Ratings?.Count(x => x.Value == Rating.Type.Dislike) ?? 0;

        public bool HasLiked(User user) => Ratings.Any(i => i.UserKey.Equals(user.Key) && i.Value == Rating.Type.Like);
        public bool HasDisliked(User user) => Ratings.Any(i => i.UserKey.Equals(user.Key) && i.Value == Rating.Type.Dislike);

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
    }
}
