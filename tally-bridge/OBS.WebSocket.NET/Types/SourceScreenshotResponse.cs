using Newtonsoft.Json;

namespace OBS.WebSocket.NET.Types
{
    /// <summary>
    /// Respone from <see cref="ObsWebSocket.TakeSourceScreenshot(string)"/>
    /// </summary>
    public class SourceScreenshotResponse
    {
        /// <summary>
        /// Source name
        /// </summary>
        [JsonProperty(PropertyName = "sourceName")]
        public string SourceName { internal set; get; }

        /// <summary>
        /// Image Data URI(if embedPictureFormat was specified in the request)
        /// </summary>
        [JsonProperty(PropertyName = "img")]
        public string ImageData { internal set; get; }

        /// <summary>
        /// Absolute path to the saved image file(if saveToFilePath was specified in the request)
        /// </summary>
        [JsonProperty(PropertyName = "imgFile")]
        public string ImageFile { internal set; get; }
    }

}
