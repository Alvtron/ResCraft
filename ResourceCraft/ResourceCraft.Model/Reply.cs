using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace ResourceCraft.Model
{
    public class Reply : FirebaseEntity, ILikeable
    {
        public FirebaseKey ContentKey { get; set; }
        public FirebaseKey UserKey { get; set; }

        private string _text;
        public string Text
        {
            get => _text;
            set => SetField(ref _text, value);
        }

        public ObservableCollection<Reply> Replies { get; set; } = new ObservableCollection<Reply>();
        public ObservableCollection<Log> Logs { get; set; } = new ObservableCollection<Log>();
        public ObservableCollection<Rating> Ratings { get; set; } = new ObservableCollection<Rating>();

        [NotMapped, JsonIgnore]
        public int NumberOfLikes => Ratings?.Count(x => x.Value == Rating.Type.Like) ?? 0;
        [NotMapped, JsonIgnore]
        public int NumberOfDislikes => Ratings?.Count(x => x.Value == Rating.Type.Dislike) ?? 0;

        public Reply()
        {
        }

        public Reply(User user, string text)
        {
            UserKey = user.Key;
            Text = text;

            Logs.Add(new Log(user.Key, "created this"));
        }

        public Reply(FirebaseKey contentKey, User user, string text)
            : this(user, text)
        {
            ContentKey = contentKey;
        }

        public void AddReply(Reply reply)
        {
            if (reply == null)
            {
                throw new ArgumentNullException(nameof(reply));
            }
            if (Replies == null)
            {
                Replies = new ObservableCollection<Reply>();
            }

            Replies.Add(reply);
            Logs.Add(new Log(reply.UserKey, "replied", reply.Key));
        }

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
