using Newtonsoft.Json;

namespace ResourceCraft.Minecraft
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Downloads
    {
        [JsonProperty("client")]
        public Client Client { get; set; }
        [JsonProperty("server")]
        public Server Server { get; set; }
    }
}
