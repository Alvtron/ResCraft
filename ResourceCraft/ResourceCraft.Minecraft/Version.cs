using Newtonsoft.Json;
using System;

namespace ResourceCraft.Minecraft
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Version : IEquatable<Version>, IComparable<Version>
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("time")]
        public DateTime Time { get; set; }
        [JsonProperty("releaseTime")]
        public DateTime ReleaseTime { get; set; }
        [JsonProperty("downloads")]
        public Downloads Downloads { get; set; }
        [JsonProperty("minimumLauncherVersion")]
        public int MinimumLauncherVersion { get; set; }

        public int CompareTo(Version other)
        {
            return ReleaseTime.CompareTo(other.ReleaseTime);
        }

        public bool Equals(Version other)
        {
            return Id.Equals(other.Id);
        }
    }
}
