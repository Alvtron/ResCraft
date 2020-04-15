using Newtonsoft.Json;
using System.Collections.Generic;

namespace ResourceCraft.Minecraft
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VersionManifest
    {
        [JsonProperty("latest")]
        public Latest Latest { get; set; }
        [JsonProperty("versions")]
        public List<VersionMinimal> Versions { get; set; }
    }
}
