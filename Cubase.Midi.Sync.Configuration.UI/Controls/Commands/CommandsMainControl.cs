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
            this.FilterBox.SelectedIndexChanged += FilterBox_SelectedIndexChanged;
            this.ClearFilter.Click += ClearFilter_Click;
        }

        private void ClearFilter_Click(object? sender, EventArgs e)
        {
            this.FilterBox.SelectedIndex = -1;
            this.commandsListView.SetAreaFilter(null);
        }

        private void FilterBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.FilterBox.SelectedIndex > -1)
            {
                this.commandsListView.SetAreaFilter(this.FilterBox.SelectedItem.ToString());
            }
        }

        public void Populate()
        {
            var cubaseServerSettings = new CubaseServerSettings();
            this.commands = cubaseServerSettings.GetCubaseCommands();
            this.FilterBox.Items.Clear();
            this.FilterBox.Items.AddRange(this.commands.GetNames().ToArray());
            this.commandsListView.Populate(this.commands, cubaseServerSettings);
        }
    }
}
