using System;
using LibAtem.Serialization;

namespace LibAtem.Commands
{
    public enum StreamState {Unknown, Starting, Stopping, Started, Stopped};

    [CommandName("SRST", CommandDirection.ToClient, 8), NoCommandId]
    public class StreamingStateCommand : SerializableCommandBase
    {
        public StreamState State { get; private set; }

        public override void Deserialize(ParsedByteArray cmd)
        {
            base.Deserialize(cmd);

            var txt = BitConverter.ToString(cmd.Body);
            if (txt.EndsWith("3B") || txt.EndsWith("04"))
                State = StreamState.Starting;

            if (txt.EndsWith("27"))
                State = StreamState.Stopped;

        }
    }

    [CommandName("StRS", CommandDirection.ToClient, 6), NoCommandId]
    public class RecordingStateCommand : SerializableCommandBase
    {
        // 0 is the 'mode' parameter, which is always 0 for now
        public override void Deserialize(ParsedByteArray cmd)
        {
            base.Deserialize(cmd);
        }
    }

    [CommandName("SRSS", CommandDirection.ToClient, 8), NoCommandId]
    public class OnAirStateCommand : SerializableCommandBase
    {
        // 0 is the 'mode' parameter, which is always 0 for now
        public override void Deserialize(ParsedByteArray cmd)
        {
            base.Deserialize(cmd);
        }
    }
}