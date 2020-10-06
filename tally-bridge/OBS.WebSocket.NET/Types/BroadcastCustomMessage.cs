using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OBS.WebSocket.NET.Types
{
    /// <summary>
    /// Custom broadcast message
    /// </summary>
    public class BroadcastCustomMessage
    {
        /// <summary>
        /// Create a custom broadcast message
        /// </summary>
        /// <param name="body"></param>
        public BroadcastCustomMessage(JObject body)
        {
            JsonConvert.PopulateObject(body.ToString(), this);
        }

        /// <summary>
        ///  	Identifier provided by the sender
        /// </summary>
        [JsonProperty(PropertyName = "realm")]
        public string Realm { set; get; }

        /// <summary>
        /// User-defined data
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public JObject Data { set; get; }
    }
}