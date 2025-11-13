using Cubase.Midi.Sync.Common.Window;
using Cubase.Midi.Sync.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.Models
{
    public class CubaseUIMixerCollection : List<CubaseUIMixer>
    {
        public CubaseUIMixerCollection() 
        { 
            this.Add(new CubaseUIMixer() { Name = "Mix 1", Indentifier = "MixConsole" });
            this.Add(new CubaseUIMixer() { Name = "Mix 2", Indentifier = "MixConsole 2" });
            this.Add(new CubaseUIMixer() { Name = "Mix 3", Indentifier = "MixConsole 3" });
            this.Add(new CubaseUIMixer() { Name = "Mix 4", Indentifier = "MixConsole 4" });
        }

        public string GetPrimaryMixerName()
        {
            return this.First().Indentifier;
        }

        public void Populate(CubaseActiveWindowCollection cubaseActiveWindows)
        {
            var matchedMixers = new HashSet<CubaseUIMixer>();

            foreach (var cubaseWindow in cubaseActiveWindows.GetAllMixers())
            {
                var mixer = this.FirstOrDefault(m => cubaseWindow.Name.StartsWith(m.Indentifier) && !matchedMixers.Any(x => x.WindowTitle == m.WindowTitle));
                if (mixer != null)
                {
                    mixer.WindowTitle = cubaseWindow.Name;
                    mixer.Window = cubaseWindow;
                    matchedMixers.Add(mixer);
                }
            }

            // Nullify any mixers that weren't matched
            foreach (var mixer in this)
            {
                if (!matchedMixers.Contains(mixer))
                {
                    mixer.WindowTitle = null;
                    mixer.Window = null;
                }
            }
        }
        
        public CubaseUIMixer GetMixerByName(string name)
        {
            return this.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) 
                   ?? new CubaseUIMixer();
        }
    }

    public class CubaseUIMixer
    {
        public string Name { get; set; } = string.Empty;

        public string Indentifier { get; set; } = string.Empty;

        public string WindowTitle  { get; set; } = string.Empty;
    
        public CubaseActiveWindow? Window { get; set; } = null;

        public CubaseWindowZOrder GetZOrder()
        {
            if (Window == null)
            {
                return CubaseWindowZOrder.Unknown;
            }
            return Window.ZOrder;
        }

        public Color GetBackgroundColour()
        {
            if (Window == null)
            {
                return System.Drawing.Color.White.ToMauiColor();
            }
            if (Window.ZOrder == CubaseWindowZOrder.Focused)
            {
                return System.Drawing.Color.Green.ToMauiColor();
            }
            return System.Drawing.Color.GreenYellow.ToMauiColor();
        }

        public Color GetForeGroundColour()
        {
            if (Window== null)
            {
                return System.Drawing.Color.SlateGray.ToMauiColor();
            }
            return System.Drawing.Color.Black.ToMauiColor();
        }

    }
}
