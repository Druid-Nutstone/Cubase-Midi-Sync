using Cubase.Midi.Sync.Common.SysEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Midi
{
    public static class MidiExtensions
    {
        public static string ToMidiString(this KnownCubaseMidiCommands command)
        {
            return command.ToString().Replace('_', ' ');
        }

        public static MidiCommand ToMidiCommand(this SysExCommand command)
        {
            return (MidiCommand)Enum.Parse(typeof(MidiCommand), command.ToString());
        }   

        public static string[] ToSingleArray(this string text)
        {
            return new string[] { text };
        }
    }
}
