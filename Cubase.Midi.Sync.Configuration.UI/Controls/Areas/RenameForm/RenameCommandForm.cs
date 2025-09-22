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

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Areas.RenameForm
{
    public partial class RenameCommandForm : Form
    {
        private CubaseCommandsCollection commands;

        private CubaseServerSettings cubaseServerSettings;

        public RenameCommandForm()
        {
            InitializeComponent();
        }

        public RenameCommandForm(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, CubaseCommandCollection cubaseCommandCollection)
        {
            InitializeComponent();
            this.commands = commands;   
            this.cubaseServerSettings = cubaseServerSettings;   
            this.ButtonOK.Click += ButtonOK_Click;
            this.CurrentNameText.Text = cubaseCommandCollection.Name;
        }

        private void ButtonOK_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewNameText.Text))
            {
                if (this.commands.Any(x => x.Name.Equals(NewNameText.Text, StringComparison.OrdinalIgnoreCase)))
                {
                    Message($"The name {NewNameText.Text} already exists");
                    return;
                }

                var currentCommandIndex = this.commands.FindIndex(x => x.Name.Equals(CurrentNameText.Text));
                this.commands[currentCommandIndex].Name = NewNameText.Text;

                foreach (var commandChild in this.commands[currentCommandIndex].Commands)
                {
                    commandChild.ParentCollectionName = NewNameText.Text;   
                }

                this.commands.SaveToFile(this.cubaseServerSettings.FilePath);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                Message("Enter a new name");
            }
        }

        private void Message(string message)
        {
            MessageBox.Show(message);
        }
    }
}
