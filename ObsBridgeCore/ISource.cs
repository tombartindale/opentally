using System;
using System.Collections.Generic;
using NLog;
using ObsBridge;

namespace ObsBridgeCore
{
    abstract class ISource
    {
        internal Logger Logger { get; }
        internal MainOptions Options { get; }
        protected ISource(Logger log, MainOptions options)
        {
            this.Logger = log;
            this.Options = options;
        }

        public abstract event Action<List<string>> OnTallyChange;
        public abstract event Action<List<string>> OnPreviewTallyChange;
        public abstract event Action<List<string>> OnSourcesChanged;
        public abstract event Action<bool> OnOnlineChanged;
        public abstract event Action<DateTime> OnRecordingStarted;
        public abstract event Action<DateTime> OnRecordingStopped;
        public abstract event Action<DateTime> OnStreamingStarted;
        public abstract event Action<DateTime> OnStreamingStopped;
        public abstract event Action<string> OnNameChanged;
        public abstract void Connect();

    }
}
