using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OBS.WebSocket.NET.Types;

namespace OBS.WebSocket.NET
{
    /// <summary>
    /// Called by <see cref="ObsWebSocket.SceneChanged"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="newSceneName">Name of the new current scene</param>
    public delegate void SceneChangeCallback(ObsWebSocket sender, string newSceneName);

    /// <summary>
    /// Called by <see cref="ObsWebSocket.SourceOrderChanged"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sceneName">Name of the scene where items where reordered</param>
    public delegate void SourceOrderChangeCallback(ObsWebSocket sender, string sceneName);

    /// <summary>
    /// Called by <see cref="ObsWebSocket.SceneItemVisibilityChanged"/>, <see cref="ObsWebSocket.SceneItemAdded"/>
    /// or <see cref="ObsWebSocket.SceneItemRemoved"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sceneName">Name of the scene where the item is</param>
    /// <param name="itemName">Name of the concerned item</param>
    public delegate void SceneItemUpdateCallback(ObsWebSocket sender, string sceneName, string itemName);

    /// <summary>
    /// Called by <see cref="ObsWebSocket.TransitionChanged"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="newTransitionName">Name of the new selected transition</param>
    public delegate void TransitionChangeCallback(ObsWebSocket sender, string newTransitionName);

    /// <summary>
    /// Called by <see cref="ObsWebSocket.TransitionDurationChanged"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="newDuration">Name of the new transition duration (in milliseconds)</param>
    public delegate void TransitionDurationChangeCallback(ObsWebSocket sender, int newDuration);

    /// <summary>
    /// Called by <see cref="ObsWebSocket.StreamingStateChanged"/>, <see cref="ObsWebSocket.RecordingStateChanged"/>
    /// or <see cref="ObsWebSocket.ReplayBufferStateChanged"/> 
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="type">New output state</param>
    public delegate void OutputStateCallback(ObsWebSocket sender, OutputState type);

    /// <summary>
    /// Called by <see cref="ObsWebSocket.StreamStatus"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="status">Stream status data</param>
    public delegate void StreamStatusCallback(ObsWebSocket sender, StreamStatus status);

    /// <summary>
    /// Called by <see cref="ObsWebSocket.StudioModeSwitched"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="enabled">New Studio Mode status</param>
    public delegate void StudioModeChangeCallback(ObsWebSocket sender, bool enabled);

    /// <summary>
    /// Called by <see cref="ObsWebSocket.Heartbeat"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="heatbeat">heartbeat data</param>
    public delegate void HeartBeatCallback(ObsWebSocket sender, Heartbeat heatbeat);

    /// <summary>
    /// Called by <see cref="ObsWebSocket.BroadcastCustomMessage"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="broadcastCustomMessage">BroadcastCustomMessage data</param>
    public delegate void BroadcastCustomMessageCallback(ObsWebSocket sender, BroadcastCustomMessage broadcastCustomMessage);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SceneItemDeselected"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sceneName">Name of the scene item was in</param>
    /// <param name="itemName">Name of the item deselected</param>
    /// <param name="itemId">Id of the item deselected</param>
    public delegate void SceneItemDeselectedCallback(ObsWebSocket sender, string sceneName, string itemName, string itemId);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SceneItemSelected"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sceneName">Name of the scene item was in</param>
    /// <param name="itemName">Name of the item seletected</param>
    /// <param name="itemId">Id of the item selected</param>
    public delegate void SceneItemSelectedCallback(ObsWebSocket sender, string sceneName, string itemName, string itemId);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SceneItemTransformChanged"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="transform">Transform data</param>
    public delegate void SceneItemTransformCallback(ObsWebSocket sender, SceneItemTransformInfo transform);


    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceAudioMixersChanged"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="mixerInfo">Mixer information that was updated</param>
    public delegate void SourceAudioMixersChangedCallback(ObsWebSocket sender, AudioMixersChangedInfo mixerInfo);



    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceAudioSyncOffsetChanged"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sourceName">Name of the source for the offset change</param>
    /// <param name="syncOffset">Sync offset value</param>
    public delegate void SourceAudioSyncOffsetCallback(ObsWebSocket sender, string sourceName, int syncOffset);


    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceCreated"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="settings">Newly created source settings</param>
    public delegate void SourceCreatedCallback(ObsWebSocket sender, SourceSettings settings);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceDestroyed"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sourceName">Newly destroyed source information</param>
    /// <param name="sourceKind">Kind of source destroyed</param>
    /// <param name="sourceType">Type of source destroyed</param>
    public delegate void SourceDestroyedCallback(ObsWebSocket sender, string sourceName, string sourceType, string sourceKind);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceRenamed"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="newName">New name of source</param>
    /// <param name="previousName">Previous name of source</param>
    public delegate void SourceRenamedCallback(ObsWebSocket sender, string newName, string previousName);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceMuteStateChanged"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sourceName">Name of source</param>
    /// <param name="muted">Current mute state of source</param>
    public delegate void SourceMuteStateChangedCallback(ObsWebSocket sender, string sourceName, bool muted);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceVolumeChanged"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sourceName">Name of source</param>
    /// <param name="volume">Current volume level of source</param>
    public delegate void SourceVolumeChangedCallback(ObsWebSocket sender, string sourceName, float volume);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceFilterVisibilityChanged"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sourceName">Name of source</param>
    /// <param name="filterName">Name of filter</param>
    /// <param name="filterEnabled">New filter state</param>
    public delegate void SourceFilterVisibilityChangedCallback(ObsWebSocket sender, string sourceName, string filterName, bool filterEnabled);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceFilterRemoved"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sourceName">Name of source</param>
    /// <param name="filterName">Name of removed filter</param>
    public delegate void SourceFilterRemovedCallback(ObsWebSocket sender, string sourceName, string filterName);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceFilterAdded"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sourceName">Name of source</param>
    /// <param name="filterName">Name of filter</param>
    /// <param name="filterType">Type of filter</param>
    /// <param name="filterSettings">Settings for filter</param>
    public delegate void SourceFilterAddedCallback(ObsWebSocket sender, string sourceName, string filterName, string filterType, JObject filterSettings);

    /// <summary>
    /// Callback by <see cref="ObsWebSocket.SourceFiltersReordered"/>
    /// </summary>
    /// <param name="sender"><see cref="ObsWebSocket"/> instance</param>
    /// <param name="sourceName">Name of source</param>
    /// <param name="filters">Current order of filters for source</param>
    public delegate void SourceFiltersReorderedCallback(ObsWebSocket sender, string sourceName, List<FilterReorderItem> filters);

    /// <summary>
    /// Thrown if authentication fails
    /// </summary>
    public class AuthFailureException : Exception
    {
    }

    /// <summary>
    /// Thrown when the server responds with an error
    /// </summary>
    public class ErrorResponseException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public ErrorResponseException(string message) : base(message)
        {
        }
    }
}
