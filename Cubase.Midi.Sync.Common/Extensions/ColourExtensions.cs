using Cubase.Midi.Sync.Common.Colours;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Extensions
{
    public static class ColourExtensions
    {
        public static SerializableColour ToSerializableColour(this System.Drawing.Color color)
        {
            return new SerializableColour
            {
                A = color.A,
                R = color.R,
                G = color.G,
                B = color.B
            };
        }

        public static Color FromSerializableColour(this SerializableColour colour)
        {
            return Color.FromArgb(colour.A, colour.R, colour.G, colour.B);
        }
    }
}
