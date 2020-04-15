using System;
using System.Collections.Generic;

namespace ResourceCraft.Model
{
    public interface IFirebaseEntity
    {
        FirebaseKey Key { get; set; }
        DateTime? Created { get; set; }
        DateTime? Updated { get; set; }
    }
}
