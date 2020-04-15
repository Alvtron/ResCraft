using Newtonsoft.Json;

namespace ResourceCraft.Minecraft
{
    /// <summary>
    /// 
    /// </summary>
    public class Pack
    {
        /// <summary>
        /// Gets or sets the pack format.
        /// </summary>
        /// <value>
        /// The pack format.
        /// </value>
        [JsonProperty("pack_format")]
        public long PackFormat { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
