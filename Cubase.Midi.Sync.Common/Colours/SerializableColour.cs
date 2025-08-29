using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Colours
{
    public class SerializableColour
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        // Optional hex string for debugging / JSON readability
        public string Hex => $"#{R:X2}{G:X2}{B:X2}{A:X2}";
    }
}
