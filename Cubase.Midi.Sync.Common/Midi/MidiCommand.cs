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
        UnSelectTracks,
        Tracks,
        TrackUpdate,
        TrackComplete,
        NextTrack,
        PreviousTrack,
        MacroCommand,
        TrackSelectionChanged,
        CommandValueChanged,
        EnableRecord,
        DisableRecord,
        CommandComplete,
        DeSelectAll,
        EnableMute,
        DisableMute,
        EnableSolo,
        DisableSolo,
        EnableListen,
        DisableListen
    }
}
