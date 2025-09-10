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
    public partial class MacroToggleDataControl : UserControl
    {
        private Control parent;
        
        public MacroToggleDataControl(Control parent = null)
        {
            InitializeComponent();
            this.parent = parent;
            this.ToogleOnAddButton.Click += ToogleOnAddButton_Click;
            this.ToggleOffAddButton.Click += ToggleOffAddButton_Click;
            this.RemoveToggleOnButton.Click += RemoveToggleOnButton_Click;
            this.RemoveToggleOffButton.Click += RemoveToggleOffButton_Click;
        }

        private void RemoveToggleOffButton_Click(object? sender, EventArgs e)
        {
            this.ToggleOffCommands.Remove();
        }

        private void RemoveToggleOnButton_Click(object? sender, EventArgs e)
        {
            this.ToggleOnCommands.Remove();
        }

        private void ToggleOffAddButton_Click(object? sender, EventArgs e)
        {
            MacroCommandInternalSelectorlForm macroCommandSelectorForm = null;
            macroCommandSelectorForm = new MacroCommandInternalSelectorlForm((selectedCubaseCommand) =>
            {
                this.ToggleOffCommands.PopulateSingle(selectedCubaseCommand.Name);
                macroCommandSelectorForm.Close();
            });

            macroCommandSelectorForm.StartPosition = FormStartPosition.Manual;

            // Align left side of child to right side of parent
            macroCommandSelectorForm.Location = new Point(
                 this.parent.Bounds.Right,   // right edge in screen coordinates
                 this.parent.Bounds.Top      // top edge in screen coordinates
            );
            macroCommandSelectorForm.Show();
        }

        private void ToogleOnAddButton_Click(object? sender, EventArgs e)
        {
            MacroCommandInternalSelectorlForm macroCommandSelectorForm = null;
            macroCommandSelectorForm = new MacroCommandInternalSelectorlForm((selectedCubaseCommand) =>
            {
                this.ToggleOnCommands.PopulateSingle(selectedCubaseCommand.Name);
                macroCommandSelectorForm.Close();
            });

            macroCommandSelectorForm.StartPosition = FormStartPosition.Manual;

            // Align left side of child to right side of parent
            macroCommandSelectorForm.Location = new Point(
                 this.parent.Bounds.Right,   // right edge in screen coordinates
                 this.parent.Bounds.Top      // top edge in screen coordinates
            );
            macroCommandSelectorForm.Show(); 
        }

        public void SetToogleOnCommands(List<string> commands)
        {
            this.ToggleOnCommands.Popsulate(commands);
        }

        public void SetToggleOffCommands(List<string> commands)
        {
            this.ToggleOffCommands.Popsulate(commands); 
        } 
        
        public List<string> GetToggleOnCommands()
        {
            return ToggleOnCommands.GetList();
        }

        public List<string> GetToggleOffCommands()
        {
            return ToggleOffCommands.GetList(); 
        }
    }
}
