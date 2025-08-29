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

        private string category;

        public KeyDataControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            cbAllocatedOnly.CheckedChanged += CbAllocatedOnly_CheckedChanged; 
        }

        private void CbAllocatedOnly_CheckedChanged(object? sender, EventArgs e)
        {
            this.Populate(this.commands, this.category);    
        }

        public void Populate(CubaseKeyCommandCollection commands, string category)
        {
            this.categoryLabel.Text = category;
            List<CubaseKeyCommand> list = commands.GetByCategory(category); 

            this.commands = commands;   
            this.category = category;   
            if (cbAllocatedOnly.Checked)
            {
                list = list.Where(x => !string.IsNullOrEmpty(x.Key)).ToList();
            }
            
            this.keysListView.Populate(list);
        }
    }
}
