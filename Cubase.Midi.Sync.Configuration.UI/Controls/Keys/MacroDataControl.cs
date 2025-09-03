using Cubase.Midi.Sync.Configuration.UI.Controls.Macros;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    public partial class MacroDataControl : UserControl
    {
        private Control parent;
        
        public MacroDataControl(Control parent = null)
        {
            InitializeComponent();
            this.parent = parent;    
            this.ButtonAddCommand.Click += ButtonAddCommand_Click;
            this.ButtonRemoveCommand.Click += ButtonRemoveCommand_Click;
        }

        private void ButtonRemoveCommand_Click(object? sender, EventArgs e)
        {
            this.stringListControl.Remove();
        }

        public List<string> GetCommands()
        {
            return this.stringListControl.GetList();
        }

        public void SetCommands(List<string> commands)
        {
            
            this.stringListControl.Popsulate(commands);
        }

        private void ButtonAddCommand_Click(object? sender, EventArgs e)
        {
            var macroCommandSelectorForm = new MacroCommandInternalSelectorlForm((selectedCubaseCommand) =>
            {
                this.stringListControl.PopulateSingle(selectedCubaseCommand.Name);
            });
            
            macroCommandSelectorForm.StartPosition = FormStartPosition.Manual;

            // Align left side of child to right side of parent
            macroCommandSelectorForm.Location = new Point(
                 this.parent.Bounds.Right,   // right edge in screen coordinates
                 this.parent.Bounds.Top      // top edge in screen coordinates
            );
            macroCommandSelectorForm.Show(); 
        }
    }
}
