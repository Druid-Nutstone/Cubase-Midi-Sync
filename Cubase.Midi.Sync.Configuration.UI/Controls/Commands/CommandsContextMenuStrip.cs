using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Commands
{
    public class CommandsContextMenuStrip : ContextMenuStrip
    {
        public CommandsContextMenuStrip(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, CommandsListView listView)
        {
            this.Items.Clear();
            this.Items.Add(new EditCommand(commands, cubaseServerSettings, listView));
        }
    }

    public class EditCommand: ToolStripMenuItem
    {
        private CubaseCommandsCollection commands;

        private CubaseServerSettings serverSettings;

        private CommandsListView listView;
        
        public EditCommand(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, CommandsListView listView)
        {
            this.commands = commands;
            this.serverSettings = cubaseServerSettings; 
            this.listView = listView;
            this.Text = "Edit";

        }

        protected override void OnClick(EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0) 
            {
                var cubaseCommandItem  = (CommandsListViewItem)this.listView.SelectedItems[0];
                var keyCommandForm = new AddKeyToCommandsForm(cubaseCommandItem.Command, commands, serverSettings, cubaseCommandItem.Category);
                keyCommandForm.ShowDialog();
            }
        }
    }
}
