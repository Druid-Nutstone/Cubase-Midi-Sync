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

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    public partial class AddKeyToCommandsForm : Form
    {
        private readonly CubaseKeyCommand cubaseKeyCommand;

        private readonly CubaseCommandsCollection cubaseCommandCollections;

        private readonly CubaseServerSettings cubaseServerSettings;

        public AddKeyToCommandsForm()
        {
            InitializeComponent();
        }

        public AddKeyToCommandsForm(CubaseKeyCommand cubaseKeyCommand, CubaseCommandsCollection cubaseCommandCollections, CubaseServerSettings cubaseServerSettings)
        {
            InitializeComponent();
            this.cubaseCommandCollections = cubaseCommandCollections;   
            this.cubaseKeyCommand = cubaseKeyCommand;   
            this.cubaseServerSettings = cubaseServerSettings;
            this.buttonName.Text = cubaseKeyCommand.Name;
            this.action.Text = cubaseKeyCommand.Key;
            this.buttonAdd.Click += ButtonAdd_Click;
            this.backgroundColourButton.Click += BackgroundColourButton_Click;
            this.textColourButton.Click += TextColourButton_Click;
        }

        private void TextColourButton_Click(object? sender, EventArgs e)
        {
            var clr = GetColour();
            this.textColour.BackColor = clr;
            this.textColour.Text = clr.ToArgb().ToString();
        }

        private void BackgroundColourButton_Click(object? sender, EventArgs e)
        {
            this.backgroundColour.Text = GetColour().ToArgb().ToString();
        }

        private Color GetColour()
        {
            var colourDialog = new ColorDialog();
            colourDialog.ShowDialog();
            return colourDialog.Color;
        }

        private void ButtonAdd_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(buttonName.Text) && !string.IsNullOrEmpty(action.Text))
            {

            }
        }
    }
}
