using Newtonsoft.Json;

namespace ResourceCraft.Minecraft
{
    /// <summary>
    /// 
    /// </summary>
    public class LANGCOUNTRY
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        [JsonProperty("region")]
        public string Region { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LANGCOUNTRY"/> is bidirectional.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bidirectional; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("bidirectional")]
        public bool Bidirectional { get; set; }
    }
}
