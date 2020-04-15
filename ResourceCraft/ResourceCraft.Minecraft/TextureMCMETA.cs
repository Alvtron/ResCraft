using Newtonsoft.Json;

namespace ResourceCraft.Minecraft
{
    /// <summary>
    /// Contains data for the texture
    /// </summary>
    public class TextureMCMETA
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Texture"/> is blur. Causes the texture to blur when viewed from close up. Defaults to false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if blur; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("blur")]
        public bool Blur { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Texture"/> is clamp. Causes the texture to stretch instead of tiling in cases where it otherwise would, such as on the shadow. Defaults to false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if clamp; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("clamp")]
        public bool Clamp { get; set; }
    }
}
