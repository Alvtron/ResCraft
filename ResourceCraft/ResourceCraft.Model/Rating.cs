using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceCraft.Model
{
    public class Rating : FirebaseEntity
    {
        public enum Type
        {
            Like,
            Dislike
        }

        public FirebaseKey UserKey { get; set; }
        public Type Value { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;

        public Rating()
        {
        }

        public Rating(User user, Type value)
        {
            UserKey = user.Key;
            Value = value;
        }
    }
}
