using System.Collections.Generic;
using Newtonsoft.Json;

namespace OBS.WebSocket.NET.Types
{
    /// <summary>
    /// Response from <see cref="ObsWebSocket.GetTransitionList"/>
    /// </summary>
    public class GetTransitionListInfo
    {
        /// <summary>
        /// Name of the currently active transition
        /// </summary>
        [JsonProperty(PropertyName = "current-transition")]
        public string CurrentTransition { set; get; }

        /// <summary>
        /// List of transitions.
        /// </summary>
        [JsonProperty(PropertyName = "transitions")]
        public List<TransitionSettings> Transitions { set; get; }
    }
}
