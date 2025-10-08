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
    public partial class MomentaryDataControl : UserControl
    {

        private ActionEvent actionEvent;
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionEvent Action 
        { 
            get
            {
                return actionEvent;
            } 
            set
            {
                if (value != null) 
                {
                    actionEvent = value;
                    action.Text = value.Action;
                }
                else
                {
                    actionEvent = new ActionEvent();
                }

            } 
        }
       
        public MomentaryDataControl()
        {
            InitializeComponent();
            action.TextChanged += Action_TextChanged;
            this.Dock = DockStyle.Fill; 
        }

        private void Action_TextChanged(object? sender, EventArgs e)
        {
            if (this.actionEvent != null)
            {
                this.actionEvent.Action = this.action.Text;
            }
        }
    }
}
