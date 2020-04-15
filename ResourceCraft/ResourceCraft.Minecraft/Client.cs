using Newtonsoft.Json;

namespace ResourceCraft.Minecraft
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Client
    {
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }
        [JsonProperty("size")]
        public int Size { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
