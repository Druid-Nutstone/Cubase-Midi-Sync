using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using System.Drawing;

namespace Cubase.Midi.Sync.UI.Extensions
{
    public static class MauiExtensions
    {
        public static Microsoft.Maui.Graphics.Color ToMauiColor(this System.Drawing.Color c)
        {
            return Microsoft.Maui.Graphics.Color.FromRgb(c.R, c.G, c.B);
        }

    }
}
