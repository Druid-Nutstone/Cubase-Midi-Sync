using Cubase.Midi.Sync.Configuration.UI.Controls.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiMacros
{
    public class MidiMacroTabControl : TabPage
    {
        private MidiMacroMainControl mainControl;   
        
        public MidiMacroTabControl() : base("Midi Macros")
        {
            BackColor = Color.FromKnownColor(KnownColor.Window);
            Controls.Clear();
            this.Controls.Clear();
            this.mainControl = new MidiMacroMainControl();
            this.Controls.Add(this.mainControl);
        }

        public void YouHaveBeenSelected()
        {
            this.mainControl.Populate();
        }
    }
}
