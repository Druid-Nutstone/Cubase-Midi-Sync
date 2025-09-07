using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Forms.CubaseCommands
{
    public class CubaseCommandsListView : ListView
    {

        public CubaseCommandsListView() : base() 
        {
            this.View = View.Details;
            this.MultiSelect = false;
            this.FullRowSelect = true;
            this.AddHeader("Binding");
            this.AddHeader("Description");
            this.AddHeader("Name");
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

        public void Populate(List<CubaseKnownCommand> cubaseKnownCommands)
        {
            this.Items.Clear();
            foreach (var command in cubaseKnownCommands)
            {
                this.Items.Add(new CubaseCommandsListViewItem(command));
            }
            this.AutoFit();
        }

    }

    public class CubaseCommandsListViewItem : ListViewItem 
    {
        public CubaseKnownCommand Command; 
        
        public CubaseCommandsListViewItem(CubaseKnownCommand cubaseKnownCommand) 
        { 
            this.Command = cubaseKnownCommand;
            this.Text = cubaseKnownCommand.CommandBinding;
            this.SubItems.Add(cubaseKnownCommand.CommandDescription);
            this.SubItems.Add(cubaseKnownCommand.CommandName);
        }
    }
}
