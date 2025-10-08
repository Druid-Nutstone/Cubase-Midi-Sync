using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Configuration.UI.Controls.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Mapping
{
    public class MappingListView : ListView
    {
       
        public MappingListView() : base() 
        { 
            this.FullRowSelect = true;
            this.View = View.Details;
            this.AddHeader("Area");
            this.AddHeader("Name");
            this.AddHeader("Action");
            this.Dock = DockStyle.Fill;
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


        public void PopulateCubaseCommand(CubaseCommand cubaseCommand)
        {
            this.Items.Add(new MappingListViewItem(cubaseCommand));
            this.AutoFit();
        }
    }

    public class MappingListViewItem : ListViewItem
    {
        public MappingListViewItem(CubaseCommand cubaseCommand)
        {
            this.Text = cubaseCommand.Category;
            this.SubItems.Add(cubaseCommand.Name);
            this.SubItems.Add(cubaseCommand.Action.Action);
        }
    }
}
