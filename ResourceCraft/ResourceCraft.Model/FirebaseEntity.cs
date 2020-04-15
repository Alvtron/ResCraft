using System;
using System.Collections.Generic;

namespace ResourceCraft.Model
{
    public abstract partial class FirebaseEntity : ObservableObject, IFirebaseEntity, IEquatable<FirebaseEntity>
    {
        public FirebaseKey Key { get; set; }
        public DateTime? Created { get; set; } = DateTime.Now;
        private DateTime? _updated = DateTime.Now;
        public DateTime? Updated
        {
            get => _updated;
            set => SetField(ref _updated, value);
        }

        public FirebaseEntity()
        {
            Key = new FirebaseKey(GetType().Name);
        }

        public bool Equals(FirebaseEntity other)
        {
            return Key.Equals(other?.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
