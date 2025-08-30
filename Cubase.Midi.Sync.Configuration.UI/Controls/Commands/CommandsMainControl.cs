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

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Commands
{
    public partial class CommandsMainControl : UserControl
    {
        private CubaseCommandsCollection commands;
        
        public CommandsMainControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        public void Populate()
        {
            var cubaseServerSettings = new CubaseServerSettings();
            this.commands = cubaseServerSettings.GetCubaseCommands();
            this.commandsListView.Populate(this.commands, cubaseServerSettings);
        }
    }
}
