using Cubase.Midi.Sync.Configuration.UI.Controls.Commands;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls
{
    public class MainTabControl : TabControl    
    {
        public MainTabControl() : base()
        {
            this.TabPages.Clear();
            this.Dock = DockStyle.Fill;
            this.TabPages.Add(new KeysTabControl());
            this.TabPages.Add(new CommandsTabControl());

        }

        protected override void OnSelected(TabControlEventArgs e)
        {
            base.OnSelected(e);
            if (e.TabPage is CommandsTabControl stp)
                stp.YouHaveBeenSelected();
        }
    }
}
