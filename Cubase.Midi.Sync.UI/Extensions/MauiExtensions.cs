using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using System.Drawing;
using Cubase.Midi.Sync.Common.Colours;

namespace Cubase.Midi.Sync.UI.Extensions
{
    public static class MauiExtensions
    {
        public static Microsoft.Maui.Graphics.Color ToMauiColor(this System.Drawing.Color c)
        {
            return Microsoft.Maui.Graphics.Color.FromRgb(c.R, c.G, c.B);
        }

        public static Microsoft.Maui.Graphics.Color ToMauiColour(this SerializableColour colour)
        {
            return Microsoft.Maui.Graphics.Color.FromRgba(
                colour.R / 255.0f,
                colour.G / 255.0f,
                colour.B / 255.0f,
                colour.A / 255.0f
            );
        }
    }
}
