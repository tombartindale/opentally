using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OBS.WebSocket.NET.Types
{
    /// <summary>
    /// Filter settings
    /// </summary>
    public class FilterSettings
    {
        /// <summary>
        /// Name of the filter
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { set; get; }

        /// <summary>
        /// Type of the specified filter
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { set; get; }

        /// <summary>
        /// Whether filter is enabled or not
        /// </summary>
        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { set; get; }

        /// <summary>
        /// Settings for the filter
        /// </summary>
        [JsonProperty(PropertyName = "settings")]
        public JObject Settings { set; get; }
    }
}