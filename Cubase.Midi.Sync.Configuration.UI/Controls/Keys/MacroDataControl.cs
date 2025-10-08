using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.InternalCommands;
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

        private List<ActionEvent> actionEvents;

        public MacroDataControl(Control parent = null)
        {
            InitializeComponent();
            this.parent = parent;    
            this.ButtonAddCommand.Click += ButtonAddCommand_Click;
            this.ButtonRemoveCommand.Click += ButtonRemoveCommand_Click;
            this.ButtonInternalCommand.OnCommand = this.AddInternalCommand;
        }

        private void ButtonRemoveCommand_Click(object? sender, EventArgs e)
        {
            this.stringListControl.Remove();
        }

        public List<ActionEvent> GetCommands()
        {
            return this.actionEvents;
        }

        public void SetCommands(List<ActionEvent> commands)
        {
            this.actionEvents = commands;
            this.stringListControl.Populate(commands);
        }

        public void AddInternalCommand(InternalCommand command)
        {
            this.stringListControl.PopulateSingle(ActionEvent.Create(CubaseAreaTypes.Midi, InternalCommandsCollection.SerialiseCommand(command)));
        }

        private void ButtonAddCommand_Click(object? sender, EventArgs e)
        {
            var macroCommandSelectorForm = new MacroCommandInternalSelectorlForm((selectedCubaseCommand) =>
            {
                this.stringListControl.PopulateSingle(ActionEvent.Create(CubaseAreaTypes.Midi, selectedCubaseCommand.Name));
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
