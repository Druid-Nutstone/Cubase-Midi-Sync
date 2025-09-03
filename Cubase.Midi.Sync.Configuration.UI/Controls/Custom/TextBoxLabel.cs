using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Custom
{
    public partial class TextBoxLabel : UserControl
    {
        public TextBoxLabel()
        {
            InitializeComponent();
            this.textBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object? sender, EventArgs e)
        {
            this.OnTextChanged(e);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Label 
        {  
            get
            {
                return this.label1.Text;
            } 
            set
            {
                this.label1.Text = value;   
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Text 
        {  
            get
            {
                return this.textBox.Text;   
            } 
            set
            {
                this.textBox.Text = value;  
            } 
        }
    }
}
