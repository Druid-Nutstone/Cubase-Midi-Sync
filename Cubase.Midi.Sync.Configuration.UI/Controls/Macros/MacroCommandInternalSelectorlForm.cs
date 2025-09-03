using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Macros
{
    public partial class MacroCommandInternalSelectorlForm : Form
    {
        public MacroCommandInternalSelectorlForm(Action<CubaseCommand> onCommandSelected)
        {
            InitializeComponent();
            this.commandsMainControl.OnCommandSelected = onCommandSelected;
            this.commandsMainControl.Populate();

        }

        
    }
}
