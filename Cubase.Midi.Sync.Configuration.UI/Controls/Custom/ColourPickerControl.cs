using Cubase.Midi.Sync.Common.Colours;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cubase.Midi.Sync.Common.Extensions;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Custom
{
    public partial class ColourPickerControl : UserControl
    {

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Label {  
            get
            {
                return this.ColourLabel.Text;
            } 
            set
            {
                this.ColourLabel.Text = value;  
            } 
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Value
        {
            get 
            { 
                return this.ColourValue.Text;
            }
            set 
            { 
                this.ColourValue.Text = value;  
            }
        }

        public SerializableColour JsonColour
        {
            get
            {
                return Color.FromArgb(int.Parse(ColourValue.Text)).ToSerializableColour();
            }
        }

        public void SetColour(SerializableColour colour)
        {
            this.ColourValue.BackColor = colour.FromSerializableColour();
            this.ColourValue.Text = this.ColourValue.BackColor.ToArgb().ToString();
        }

        public ColourPickerControl()
        {
            InitializeComponent();
            ColourPickerButton.Click += ColourPickerButton_Click;
        }

        private void ColourPickerButton_Click(object? sender, EventArgs e)
        {
            var colourDialog = new ColorDialog();
            if (colourDialog.ShowDialog() == DialogResult.OK) 
            {
                this.ColourValue.BackColor = colourDialog.Color;
                this.ColourValue.Text = colourDialog.Color.ToArgb().ToString();
            }
        }
    }
}
