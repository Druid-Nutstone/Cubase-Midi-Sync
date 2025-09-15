using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Midi
{
    public enum MidiCommand
    {
        ClearChannels,
        ChannelChange,
        Message,
        Ready,
        SelectTracks,
        Tracks,
        TrackUpdate,
        TrackComplete,
        NextTrack,
        PreviousTrack
    }
}
