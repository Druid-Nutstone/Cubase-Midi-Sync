using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.InternalCommands;
using Cubase.Midi.Sync.Configuration.UI.Controls.Macros;
using Cubase.Midi.Sync.Configuration.UI.Controls.MidiAndKeys;
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
            return this.stringListControl.GetCommands();
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
            var parentForm = this.GetParentForm(this);
            MidiAndKeysForm form;
            form = new MidiAndKeysForm((key) =>
            {
                this.stringListControl.PopulateSingle(ActionEvent.CreateFromMidiAndKey(key));
            });
            form.StartPosition = FormStartPosition.Manual;
            form.CloseAfterSelect = true;
            parentForm.Move += (sender, e) =>
            {
                form.Location = new Point(
                    parentForm.Bounds.Right,   // right edge in screen coordinates
                    parentForm.Bounds.Top      // top edge in screen coordinates
               );
            };

            // Align left side of child to right side of parent
            form.Location = new Point(
                 parentForm.Bounds.Right,   // right edge in screen coordinates
                 parentForm.Bounds.Top      // top edge in screen coordinates
            );
            form.Show();
        }

        private Control GetParentForm(Control control)
        {
            var cntrl = control;
            while (cntrl.GetType() != typeof(AddKeyToCommandsForm))
            {
                if (cntrl.Parent != null)
                {
                    cntrl = cntrl.Parent;
                }
                else
                {
                    return cntrl;
                }
            }
            return cntrl;
        }
    }
}
