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
            this.ToggleOnInternalCommand.OnCommand = this.AddInternalCommand; 
            this.ToggleOffInternalCommand.OnCommand = this.AddInternalOffCommand;
        }

        private void AddInternalCommand(InternalCommand command)
        {
            this.ToggleOnCommands.PopulateSingle(ActionEvent.Create(CubaseAreaTypes.Midi,InternalCommandsCollection.SerialiseCommand(command)));
        }

        private void AddInternalOffCommand(InternalCommand command)
        {
            this.ToggleOffCommands.PopulateSingle(ActionEvent.Create(CubaseAreaTypes.Midi,InternalCommandsCollection.SerialiseCommand(command))) ;
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
            var parentForm = this.GetParentForm(this);
            MidiAndKeysForm form;
            form = new MidiAndKeysForm((key) =>
            {
                this.ToggleOffCommands.PopulateSingle(ActionEvent.CreateFromMidiAndKey(key));
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

        private void ToogleOnAddButton_Click(object? sender, EventArgs e)
        {
            var parentForm = this.GetParentForm(this);
            MidiAndKeysForm form;
            form = new MidiAndKeysForm((key) =>
            {
                this.ToggleOnCommands.PopulateSingle(ActionEvent.CreateFromMidiAndKey(key));
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

        public void SetToogleOnCommands(List<ActionEvent> commands)
        {
            this.ToggleOnCommands.Populate(commands);
        }

        public void SetToggleOffCommands(List<ActionEvent> commands)
        {
            this.ToggleOffCommands.Populate(commands); 
        } 
        
        public List<ActionEvent> GetToggleOnCommands()
        {
            return this.ToggleOnCommands.GetCommands();
        }

        public List<ActionEvent> GetToggleOffCommands()
        {
            return this.ToggleOffCommands.GetCommands(); 
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
