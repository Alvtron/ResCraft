using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceCraft.Minecraft
{
    /// <summary>
    /// 
    /// </summary>
    public class MCMETA
    {
        /// <summary>
        /// Gets or sets the animation.
        /// </summary>
        /// <value>
        /// The animation.
        /// </value>
        [JsonProperty("animation")]
        public Animation Animation { get; set; }
        /// <summary>
        /// Gets or sets the pack.
        /// </summary>
        /// <value>
        /// The pack.
        /// </value>
        [JsonProperty("pack")]
        public Pack Pack { get; set; }
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        [JsonProperty("language")]
        public Language Language { get; set; }
        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        [JsonProperty("texture")]
        public TextureMCMETA Texture { get; set; }
    }
}
