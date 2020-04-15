using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using ResourceCraft.Minecraft;

namespace ResourceCraft.Minecraft
{

    public class ResourceRelation : Dictionary<string, string>
    {
        [JsonIgnore]
        new public ICollection<string> Keys => base.Keys;
        [JsonIgnore]
        new public ICollection<string> Values => base.Values;
        [JsonIgnore]
        new public int Count => Count;

        public ResourceRelation()
            : base(new VersionComparer())
        {
        }

        public void Add(IResource resource)
        {
            Add(resource.Version, resource.Path);
        }
    }
}
