using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    public class KeysListView : ListView
    {
        private CubaseCommandsCollection commands;
        
        public KeysListView() : base()
        {
            var cubaseServerSettings = new CubaseServerSettings();
            this.commands = cubaseServerSettings.GetCubaseCommands();
            this.FullRowSelect = true;
            this.MultiSelect = true;
            this.View = View.Details;
            this.Dock = DockStyle.Fill;
            this.AddHeader("Name");
            this.AddHeader("Key");
            this.ContextMenuStrip = new KeysContentMenuStrip(this.commands, this, cubaseServerSettings);
        }

        public void AddHeader(string header)
        {
            this.Columns.Add(header);
        }

        public void Populate(List<CubaseKeyCommand> commands)
        {
            this.Items.Clear();
            commands.ForEach(c => this.Items.Add(new KeysListViewItem(c)));
            this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            this.Columns[Columns.Count - 1].Width = -2; // auto size last column
        }
    }

    public class KeysListViewItem : ListViewItem
    {
        public CubaseKeyCommand Command { get; private set; }   

        public KeysListViewItem(CubaseKeyCommand command)
        {
            this.Command = command; 
            this.Text = command.Name;
            this.SubItems.Add(command.Key);
        }
    }
}
