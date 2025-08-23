using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Sync.Midi.Midi
{
    public class MidiController
    {
        private const int PageSelectCC = 0; // Must match PAGE_SELECT_CC in JS driver

        private readonly MidiManager midi;
        private readonly MidiMapper mapper;

        private MidiPage? activePage;

        public MidiController(MidiManager midi, MidiMapper mapper)
        {
            this.midi = midi;
            this.mapper = mapper;

            if (mapper.Pages.Count > 0)
                activePage = mapper.Pages[0]; // default to first page
        }

        public void SendPlay()
        {
            // Note On, channel 1 (0x90), note 0x5E, velocity 127
            byte[] msg = new byte[] { 0x90, 0x5E, 0x7F };
            midi.SendMessage(msg);
        }

        public void SendCC(int channel, int cc, int value)
        {
            // CC message: 0xB0 = Control Change base for channel 0
            byte status = (byte)(0xB0 | (channel & 0x0F));
            midi.SendMessage(new byte[] { status, (byte)cc, (byte)value });
        }

        public void SendNote(int channel, int note, int velocity)
        {
            // Note On: 0x90 base for channel 0
            byte status = (byte)(0x90 | (channel & 0x0F));
            midi.SendMessage(new byte[] { status, (byte)note, (byte)velocity });
        }

        // --- Receive and map incoming CCs ---
        public (string controlName, int value)? TryMapIncoming(byte[] msg)
        {
            if (msg.Length < 2 || activePage == null) return null;

            int cc = msg[0];
            int value = msg[1];

            var btn = activePage.Buttons.FirstOrDefault(b => b.CC == cc);
            if (btn != null) return (btn.Name, value);

            var fader = activePage.Faders.FirstOrDefault(f => f.CC == cc);
            if (fader != null) return (fader.Name, value);

            var knob = activePage.Knobs.FirstOrDefault(k => k.CC == cc);
            if (knob != null) return (knob.Name, value);

            return null;
        }
    }


}
