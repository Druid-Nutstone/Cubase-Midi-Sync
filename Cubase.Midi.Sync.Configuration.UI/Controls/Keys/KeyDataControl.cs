using Cubase.Midi.Sync.Common.Keys;
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
    public partial class KeyDataControl : UserControl
    {
        private CubaseKeyCommandCollection commands;

        public KeyDataControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            cbAllocatedOnly.CheckedChanged += CbAllocatedOnly_CheckedChanged; 
        }

        private void CbAllocatedOnly_CheckedChanged(object? sender, EventArgs e)
        {
            this.Populate(this.commands);    
        }

        public void Populate(CubaseKeyCommandCollection commands)
        {
           
            this.keysListView.Populate(commands);
        }
    }
}
