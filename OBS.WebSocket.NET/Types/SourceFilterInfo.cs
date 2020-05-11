using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OBS.WebSocket.NET.Types
{
    /// <summary>
    /// OBS Source Filter info
    /// </summary>
    public class SourceFilterInfo
    {
        /// <summary>
        /// Filter status (enabled or not)
        /// </summary>
        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { internal set; get; }

        /// <summary>
        /// Filter type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { internal set; get; }

        /// <summary>
        /// Filter name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { internal set; get; }

        /// <summary>
        /// Filter settings
        /// </summary>
        [JsonProperty(PropertyName = "settings")]
        public JObject Settings { internal set; get; }
    }
}