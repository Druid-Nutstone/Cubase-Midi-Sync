using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using Cubase.Midi.Sync.Configuration.UI.Controls.Midi;
using Cubase.Midi.Sync.Configuration.UI.Controls.MidiMacros.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiMacros
{
    public partial class MidiMacroMainControl : UserControl
    {
        private CubaseServerSettings cubaseServerSettings;

        private CubaseMacroCollection commands;  

        public MidiMacroMainControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.AddMacroButton.Click += AddMacroButton_Click;
        }

        private void AddMacroButton_Click(object? sender, EventArgs e)
        {
            var parentForm = this.GetParentForm(this);
            AddMidiMacroForm form;
            form = new AddMidiMacroForm((key) =>
            {
                if (!this.commands.Any(x => x.Name == key.Name))
                {
                    //this.commands.AddMacro(key.Name);
                    //this.commands.SaveToFile(this.cubaseServerSettings.MacroFilePath);
                    //this.Populate();
                }
                else
                {
                    MessageBox.Show($"That macro {key.Name} is already selected");
                }
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

        public void Populate()
        {
            this.cubaseServerSettings = new CubaseServerSettings();
            //this.commands = cubaseServerSettings.GetMacros();
            this.midiMacroListView.Populate(this.commands);
        }
    }
}
