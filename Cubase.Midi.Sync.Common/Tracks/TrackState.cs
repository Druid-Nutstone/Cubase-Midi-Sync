using Cubase.Midi.Sync.Common.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Tracks
{
    public class TrackState
    {
        public int TrackCount { get; set; } = 0;

        public bool TracksComplete { get; set; } = false;


        public static TrackState CreateFromChannels(MidiChannelCollection midiChannels)
        {
            return new TrackState 
            { 
                TrackCount = midiChannels.Count, 
                TracksComplete = midiChannels.Count == 24 
            };
        }
    }
}
