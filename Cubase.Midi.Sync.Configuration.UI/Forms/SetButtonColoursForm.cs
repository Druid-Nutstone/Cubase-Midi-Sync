using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Forms
{
    public partial class SetButtonColoursForm : Form
    {
        public SetButtonColoursForm()
        {
            InitializeComponent();
            ButtonExampleControl.TestButtonText = "Example Button";
            ButtonExampleControl.BackgroundColourPicker = this.BackgroundColour;
            ButtonExampleControl.TextColourPicker = this.TextColour;
            ButtonOK.Click += ButtonOK_Click;
            ButtonExampleControl.Initialise();
            UseDefaultsCheckBox.CheckedChanged += UseDefaultsCheckBox_CheckedChanged;
        }

        private void UseDefaultsCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            this.BackgroundColour.SetColour(ColourConstants.ButtonBackground.ToSerializableColour());
            this.TextColour.SetColour(ColourConstants.ButtonText.ToSerializableColour());
        }

        private void ButtonOK_Click(object? sender, EventArgs e)
        {
            var cubaseServerSettings = new CubaseServerSettings();
            var commands = cubaseServerSettings.GetCubaseCommands();
            foreach (var command in commands)
            {
                if (command.Visible)
                {
                    foreach (var button in command.Commands)
                    {
                        button.ButtonBackgroundColour = this.BackgroundColour.JsonColour;
                        button.ButtonTextColour = this.TextColour.JsonColour;  
                    }
                }
            }
            commands.SaveToFile(cubaseServerSettings.FilePath);
            this.Close();
        }
    }
}
