using Cubase.Midi.Sync.Common.Keys;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
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
    public partial class MacroCommandSelectorForm : Form
    {
        private Action<CubaseKeyCommand> keyHandkler;

        private CubaseKeyCommandCollection cubaseKeyCommands;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CloseAfterSelect { get; set; } = false;

        public MacroCommandSelectorForm()
        {
            InitializeComponent();
        }

        public MacroCommandSelectorForm(Action<CubaseKeyCommand> keyHandler)
        {
            InitializeComponent();
            this.searchFilter.KeyPress += SearchFilter_KeyPress;
            this.searchFilter.TextChanged += SearchFilter_TextChanged;
            this.keyHandkler = keyHandler;
            this.cubaseKeyCommands = CubaseKeyCommandParser.Create().Parse();
            this.commandSelectorListView.Populate(this.cubaseKeyCommands.GetAllocated(), (key) => 
            {
                keyHandkler(key);
                if (this.CloseAfterSelect)
                {
                    this.Close();
                }
            });
        }

        private void SearchFilter_TextChanged(object? sender, EventArgs e)
        {
            if (this.searchFilter.Text.Length > 3)
            {
                var commands = this.cubaseKeyCommands.GetByName(this.searchFilter.Text);
                commands.AddRange(this.cubaseKeyCommands.GetByKey(this.searchFilter.Text));
                commands.AddRange(this.cubaseKeyCommands.GetByCubaseDescription(this.searchFilter.Text));
                this.commandSelectorListView.Populate(commands.Distinct().ToList(), this.keyHandkler);
            }
        }

        private void SearchFilter_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
            {
                this.commandSelectorListView.Populate(this.cubaseKeyCommands.GetByName(searchFilter.Text), this.keyHandkler);
            }
        }
    }
}
