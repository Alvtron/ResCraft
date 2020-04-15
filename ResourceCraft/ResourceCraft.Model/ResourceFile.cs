using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ResourceCraft.Model
{

    public class ResourceFile : FirebaseFile
    {
        public FirebaseKey ResourceRelationKey { get; set; }

        public ResourceFile() { }

        public ResourceFile(FirebaseKey resourceRelationKey)
        {
            ResourceRelationKey = resourceRelationKey ?? throw new ArgumentNullException(nameof(resourceRelationKey));
        }
    }
}
