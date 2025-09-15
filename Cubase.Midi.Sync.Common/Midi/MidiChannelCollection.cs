using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Midi
{
    public class MidiChannelCollection : List<MidiChannel>
    {
        
        public List<MidiChannel> GetActiveChannels()
        {
            return this.Where(x => !string.IsNullOrEmpty(x.Name)).ToList();
        }

        public MidiChannel GetChannelByName(string name)
        {
            return this.FirstOrDefault(x => x.Name != null && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public MidiChannel GetSelectedTrack()
        {
            return this.FirstOrDefault(x => x.Selected.HasValue && x.Selected.Value);
        }

        public MidiChannel AddOrUpdateChannel(MidiChannel midiChannel)
        {
            var channel = this.FirstOrDefault(x => x.Index == midiChannel.Index);
            if (channel != null)
            {
                if (!string.IsNullOrEmpty(midiChannel.Name))
                {
                    channel.Name = midiChannel.Name;
                }
                if (midiChannel.Volume.HasValue)
                {
                    channel.Volume = channel.Volume;
                }
                if (midiChannel.RecordEnable.HasValue)
                {
                    channel.RecordEnable = midiChannel.RecordEnable;
                }
                if (midiChannel.Mute.HasValue)
                {
                    channel.Mute = midiChannel.Mute;
                }
                if (midiChannel.Solo.HasValue)
                {
                    channel.Solo = midiChannel.Solo;
                }
                if (midiChannel.Selected.HasValue)
                {
                    channel.Selected = midiChannel.Selected;
                }   
            }
            else
            {
                this.Add(midiChannel);
                channel = midiChannel;
            }
            return channel;
        }
    }

    public class MidiChannel
    {
        public string? Name { get; set; } = null;

        public int Index { get; set; }

        public float? Volume { get; set; } = 0;

        public bool? RecordEnable { get; set; } = null;

        public bool? Mute { get; set; } = null;    

        public bool? Solo { get; set; } = null;

        public bool? Selected { get; set; } = null;
    }
}
