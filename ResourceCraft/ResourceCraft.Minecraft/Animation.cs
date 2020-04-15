using Newtonsoft.Json;
using System.Collections.Generic;

namespace ResourceCraft.Minecraft
{
    /// <summary>
    /// Contains data for the animation
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Animation" /> is interpolate.
        /// If true, Minecraft will generate additional frames between frames with a frame time greater than 1 between them.
        /// Defaults to false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if interpolate; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("interpolate")]
        public bool Interpolate { get; set; }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        /// The width of the tile, as a direct ratio rather than in pixels. This is unused in vanilla but can be used by mods to have frames that are not perfect squares.
        [JsonProperty("width")]
        public int Width { get; set; }
        /// <summary>
        /// Gets or sets the height. The height of the tile in direct pixels, as a ratio rather than in pixels. This is unused in vanilla but can be used by mods to have frames that are not perfect squares.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        [JsonProperty("height")]
        public int Height { get; set; }
        /// <summary>
        /// Gets or sets the frametime. Sets the default time for each frame in increments of one game tick. Defaults to 1.
        /// </summary>
        /// <value>
        /// The frametime.
        /// </value>
        [JsonProperty("frametime")]
        public int Frametime { get; set; }
        /// <summary>
        /// Gets or sets the frames. Contains a list of frames. Defaults to displaying all the frames from top to bottom.
        /// </summary>
        /// <value>
        /// The frames.
        /// </value>
        [JsonProperty("frames")]
        public List<Frame> Frames { get; set; }
    }
}
