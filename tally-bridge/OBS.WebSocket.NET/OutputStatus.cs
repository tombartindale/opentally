﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OBS.WebSocket.NET
{
    /// <summary>
    /// Status of streaming output and recording output
    /// </summary>
    public class OutputStatus
    {
        /// <summary>
        /// True if streaming is started and running, false otherwise
        /// </summary>
        [JsonProperty(PropertyName = "streaming")]

        public readonly bool IsStreaming;

        /// <summary>
        /// True if recording is started and running, false otherwise
        /// </summary>
        [JsonProperty(PropertyName = "recording")]
        public readonly bool IsRecording;

        /// <summary>
        /// True if recording is paused, false otherwise
        /// </summary>
        [JsonProperty(PropertyName = "recording-paused")]
        public readonly bool IsPaused;

        /// <summary>
        /// Builds the object from the JSON response body
        /// </summary>
        /// <param name="data">JSON response body as a <see cref="JObject"/></param>
        public OutputStatus(JObject data)
        {
            JsonConvert.PopulateObject(data.ToString(), this);
        }
    }
}
