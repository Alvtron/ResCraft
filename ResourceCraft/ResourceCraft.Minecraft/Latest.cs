using Newtonsoft.Json;

namespace ResourceCraft.Minecraft
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Latest
    {
        [JsonProperty("snapshot")]
        public string Snapshot { get; set; }
        [JsonProperty("release")]
        public string Release { get; set; }
    }
}
