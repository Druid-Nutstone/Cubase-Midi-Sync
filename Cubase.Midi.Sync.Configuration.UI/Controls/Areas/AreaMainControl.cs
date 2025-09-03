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

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Areas
{
    public partial class AreaMainControl : UserControl
    {
        private CubaseServerSettings cubaseServerSettings;
        
        private CubaseCommandsCollection commands;

        public AreaMainControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill; 
        }

        public void Populate()
        {
            this.cubaseServerSettings = new CubaseServerSettings();
            this.commands = cubaseServerSettings.GetCubaseCommands();
            this.areaListView.Populate(this.commands, this.cubaseServerSettings);
        
        }
    }
}
