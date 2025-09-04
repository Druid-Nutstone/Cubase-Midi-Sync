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
            this.AddHeader("Cubase Description");
            this.AddHeader("Cubase Command");
            this.AddHeader("Cubase Area");
            this.ContextMenuStrip = new KeysContentMenuStrip(this.commands, this, cubaseServerSettings);
        }

        public void AddHeader(string header)
        {
            this.Columns.Add(header);
        }

        private void AutoFit()
        {
            foreach (ColumnHeader column in this.Columns)
            {
                // Measure header width
                this.AutoResizeColumn(column.Index, ColumnHeaderAutoResizeStyle.HeaderSize);
                int headerWidth = column.Width;

                // Measure content width
                this.AutoResizeColumn(column.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
                int contentWidth = column.Width;

                // Pick whichever is larger
                column.Width = Math.Max(headerWidth, contentWidth);
            }
        }

        public void Populate(List<CubaseKeyCommand> commands)
        {
            this.Items.Clear();
            commands.ForEach(c => this.Items.Add(new KeysListViewItem(c)));
            this.AutoFit();
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
            this.SubItems.Add(command.CubaseCommand?.CommandDescription);
            this.SubItems.Add(command.CubaseCommand?.CommandName);
            this.SubItems.Add(command.CubaseCommand?.CommandBinding);
        }
    }
}
