using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common
{
    public static class CubaseServiceConstants
    {
        public static string KeyService = "Keys";

        public static string MidiService = "Midi";

        public static string MidiMacroService = "MidiMacro";

        public static string GenericMidiFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steinberg", "Cubase 15_64", "Generic Remote.xml");
    }

    public enum CubaseAreaTypes
    {
        Keys,
        Midi,
        MidiMacro,
        KeyMacro,
        Preferences,
        Script
    }
}
