using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceCraft.Model
{
    public class FirebaseKey : IEquatable<FirebaseKey>
    {
        public string Uid { get; set; }

        public string Group { get; set; }

        public FirebaseKey() { }

        public FirebaseKey(string key, string group)
        {
            Uid = key;
            Group = group;
        }

        public FirebaseKey(string group)
        {
            Uid = Guid.NewGuid().ToString();
            Group = group;
        }

        public bool Equals(FirebaseKey other)
        {
            if (other == null)
            {
                return false;
            }
            return Uid.Equals(other.Uid) && Group.Equals(other.Group);
        }

        public override string ToString()
        {
            return Uid;
        }
    }
}
