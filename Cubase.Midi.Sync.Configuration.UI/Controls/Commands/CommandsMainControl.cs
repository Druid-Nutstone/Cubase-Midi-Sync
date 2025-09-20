using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Keys;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using Cubase.Midi.Sync.Configuration.UI.Controls.Macros;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Commands
{
    public partial class CommandsMainControl : UserControl
    {
        private CubaseCommandsCollection commands;

        private CubaseServerSettings cubaseServerSettings;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<CubaseCommand> OnCommandSelected { get; set; }    

        public CommandsMainControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.FilterBox.SelectedIndexChanged += FilterBox_SelectedIndexChanged;
            this.ClearFilter.Click += ClearFilter_Click;
            this.AddKeyCommandButton.Click += AddKeyCommandButton_Click;
            this.AddNewButton.Click += AddNewButton_Click;
            this.AddMidiCommandButton.Click += AddMidiCommandButton_Click;
        }

        private void AddMidiCommandButton_Click(object? sender, EventArgs e)
        {
            var parentForm = this.GetParentForm(this);
            MidiCommandSelectorForm form;
            form = new MidiCommandSelectorForm((key) =>
            {
                var cubaseKeyCommand = new CubaseKeyCommand()
                {
                    Name = key.Name,
                    Category = key.Category,
                    Action = key.Command,
                    Key = key.Command 
                };
                var keyCommandForm = new AddKeyToCommandsForm(cubaseKeyCommand, commands, cubaseServerSettings);
                keyCommandForm.ShowDialog();
                this.Populate();
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

        private void AddNewButton_Click(object? sender, EventArgs e)
        {
            var keyCommandForm = new AddKeyToCommandsForm(CubaseKeyCommand.Create(), commands, cubaseServerSettings);
            keyCommandForm.ShowDialog();
            this.Populate();
        }

        private void AddKeyCommandButton_Click(object? sender, EventArgs e)
        {
            var parentForm = this.GetParentForm(this);
            MacroCommandSelectorForm form;
            form = new MacroCommandSelectorForm((key) => 
            {
                var keyCommandForm = new AddKeyToCommandsForm(key, commands, cubaseServerSettings);
                keyCommandForm.ShowDialog();
                this.Populate();
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

        private void ClearFilter_Click(object? sender, EventArgs e)
        {
            this.FilterBox.SelectedIndex = -1;
            this.commandsListView.SetAreaFilter(null);
        }

        private void FilterBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.FilterBox.SelectedIndex > -1)
            {
                this.commandsListView.SetAreaFilter(this.FilterBox.SelectedItem.ToString());
            }
        }

        public void Populate()
        {
            this.cubaseServerSettings = new CubaseServerSettings();
            this.commands = cubaseServerSettings.GetCubaseCommands();
            this.FilterBox.Items.Clear();
            this.FilterBox.Items.AddRange(this.commands.GetNames().ToArray());
            this.commandsListView.OnCommandSelected = this.OnCommandSelected;
            this.commandsListView.Populate(this.commands, cubaseServerSettings);
             
        }

    }
}
