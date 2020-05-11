using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OBS.WebSocket.NET.Types
{
    /// <summary>
    /// Output information
    /// </summary>
    public class OutputInfo
    {
        /// <summary>
        /// Name of output
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { set; get; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { set; get; }

        /// <summary>
        /// Settings object
        /// </summary>
        [JsonProperty(PropertyName = "settings")]
        public JObject Settings { set; get; }

        /// <summary>
        /// Active state
        /// </summary>
        [JsonProperty(PropertyName = "active")]
        public bool Active { set; get; }

        /// <summary>
        /// Congestion value
        /// </summary>
        [JsonProperty(PropertyName = "congestion")]
        public double Congestion { set; get; }
    }
}