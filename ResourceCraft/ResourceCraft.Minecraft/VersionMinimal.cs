using Newtonsoft.Json;
using System;

namespace ResourceCraft.Minecraft
{
    [JsonObject(MemberSerialization.OptIn)]
    public class VersionMinimal
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("time")]
        public DateTime Time { get; set; }
        [JsonProperty("releaseTime")]
        public DateTime ReleaseTime { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
