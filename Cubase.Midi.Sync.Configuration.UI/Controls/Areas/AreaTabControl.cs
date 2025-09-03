using Cubase.Midi.Sync.Configuration.UI.Controls.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Areas
{
    public class AreaTabControl : TabPage 
    {
        private AreaMainControl mainControl;
        
        public AreaTabControl() : base("Area Maintenance")
        {
            BackColor = Color.FromKnownColor(KnownColor.Window);
            Controls.Clear();
            this.Controls.Clear();
            this.mainControl = new AreaMainControl();
            this.mainControl.Populate();
            this.Controls.Add(this.mainControl);
        }

        public void YouHaveBeenSelected()
        {
            this.mainControl.Populate();
        }
    }
}
