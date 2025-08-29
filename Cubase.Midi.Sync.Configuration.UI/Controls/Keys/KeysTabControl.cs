using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    public class KeysTabControl : TabPage
    {
        public KeysTabControl() : base("Keys")
        {
            BackColor = Color.FromKnownColor(KnownColor.Window);
            Controls.Clear();
            Controls.Add(new KeysMainControl());
        }
    }
}
