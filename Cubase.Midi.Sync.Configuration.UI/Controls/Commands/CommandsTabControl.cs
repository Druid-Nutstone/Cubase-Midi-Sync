using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Commands
{
    public class CommandsTabControl : TabPage
    {
        private CommandsMainControl mainControl;
        
        public CommandsTabControl() : base("Cubase Commands")
        {
            BackColor = Color.FromKnownColor(KnownColor.Window);
            Controls.Clear();
            this.Controls.Clear();
            this.mainControl = new CommandsMainControl();
            this.mainControl.Populate();
            this.Controls.Add(this.mainControl);
        }

        public void YouHaveBeenSelected()
        {
            this.mainControl.Populate();
        }


    }
}
