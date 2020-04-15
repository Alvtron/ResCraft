using Newtonsoft.Json;

namespace ResourceCraft.Minecraft
{
    // Specifies a frame with additional data
    /// <summary>
    /// 
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// Gets or sets the index. A number corresponding to position of a frame from the top, with the top frame being 0.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        [JsonProperty("index")]
        public int Index { get; set; }
        /// <summary>
        /// Gets or sets the time. The time in ticks to show this frame, overriding "frametime" above.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        [JsonProperty("time")]
        public int Time { get; set; }
    }
}
