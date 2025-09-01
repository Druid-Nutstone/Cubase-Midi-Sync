using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Custom
{
    public partial class ButtonExampleControl : UserControl
    {

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SerializableColour TestBackColour { get; set; } = Color.White.ToSerializableColour();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SerializableColour TestTextColour { get; set; } = Color.White.ToSerializableColour();


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TestButtonText 
        { 
            get
            {
                return this.ExampleButton.Text;
            } 
            set
            {
                this.ExampleButton.Text = value;   
            } 
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColourPickerControl BackgroundColourPicker { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColourPickerControl TextColourPicker { get; set; }   

        public ButtonExampleControl()
        {
            InitializeComponent();

        }


        public void Initialise()
        {
            SetColours();

            this.BackgroundColourPicker.ColourChanged = (c) => 
            {
                this.TestBackColour = c;
                this.SetColours(); 
            };

            this.TextColourPicker.ColourChanged = (c) => 
            {
                this.TestTextColour = c;
                this.SetColours();
            };
        
        }

        private void SetColours()
        {
            this.ExampleButton.BackColor = TestBackColour.FromSerializableColour();
            this.ExampleButton.ForeColor = TestTextColour.FromSerializableColour();
        }
    }
}
