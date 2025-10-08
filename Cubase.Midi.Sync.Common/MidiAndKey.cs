using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common
{
    public class MidiAndKey
    {
        public CubaseAreaTypes KeyType { get; set; }

        public string Name { get; set; }

        public string Action { get; set; }

        public string Category { get; set; }    

        public static MidiAndKey AddKey(string name, string action, string category)
        {
            return new MidiAndKey() { KeyType = category == "Macro" ? CubaseAreaTypes.KeyMacro : CubaseAreaTypes.Keys, Name = name, Action = action, Category = category };
        }

        public static MidiAndKey AddMidi(string name, string action, string category)
        {
            return new MidiAndKey() { KeyType = CubaseAreaTypes.Midi, Name = name, Action = action, Category = category };
        }
    }
}
