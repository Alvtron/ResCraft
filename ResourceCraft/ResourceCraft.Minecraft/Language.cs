using Newtonsoft.Json;

namespace ResourceCraft.Minecraft
{
    /// <summary>
    /// 
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Gets or sets the language country.
        /// </summary>
        /// <value>
        /// The language country.
        /// </value>
        [JsonProperty("LANG_COUNTRY")]
        public LANGCOUNTRY LANG_COUNTRY { get; set; }
    }
}
