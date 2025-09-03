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
    public partial class MomentaryDataControl : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Text 
        { 
            get
            {
                return action.Text;
            } 
            set
            {
                action.Text = value;    
            } 
        }
       
        public MomentaryDataControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill; 
        }
    }
}
