using Cubase.Midi.Sync.Configuration.UI.Controls.Areas;
using Cubase.Midi.Sync.Configuration.UI.Controls.Commands;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using Cubase.Midi.Sync.Configuration.UI.Controls.Mapping;
using Cubase.Midi.Sync.Configuration.UI.Controls.MidiMacros;
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
            this.TabPages.Add(new CommandsTabControl());
            this.TabPages.Add(new MappingTabControl());
            this.TabPages.Add(new AreaTabControl());
            this.TabPages.Add(new KeysTabControl());
        }

        protected override void OnSelected(TabControlEventArgs e)
        {
            base.OnSelected(e);
            if (e.TabPage is CommandsTabControl stp)
                stp.YouHaveBeenSelected();
            if (e.TabPage is MappingTabControl mapping)
                mapping.YouHaveBeenSelected();
            if (e.TabPage is AreaTabControl area)
                area.YouHaveBeenSelected();
        }
    }
}
