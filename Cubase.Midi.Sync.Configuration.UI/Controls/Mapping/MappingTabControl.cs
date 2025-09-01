using Cubase.Midi.Sync.Configuration.UI.Controls.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Mapping
{
    public class MappingTabControl : TabPage
    {
        private MappingMainControl mainControl;
        
        public MappingTabControl() : base("Map Commands To Area")
        {
            BackColor = Color.FromKnownColor(KnownColor.Window);
            Controls.Clear();
            this.Controls.Clear();
            this.mainControl = new MappingMainControl();
            this.Controls.Add(this.mainControl);
        }
        
        public void YouHaveBeenSelected()
        {
            this.mainControl.Populate();
        }
    }
}
