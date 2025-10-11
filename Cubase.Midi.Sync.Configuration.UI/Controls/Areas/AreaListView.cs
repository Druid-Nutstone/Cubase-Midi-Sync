using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Areas
{
    public class AreaListView : ListView
    {
        private CubaseCommandsCollection cubaseCommands;
        private CubaseServerSettings cubaseServerSettings;

        private Action<CubaseCommandCollection> selectionHandler;

        public AreaListView() : base() 
        {
            this.View = View.Details;
            this.DoubleBuffered = true;
            this.MultiSelect = false;
            this.FullRowSelect = true;
            this.AddHeader("Area Name");
            this.AddHeader("Visible");
            this.AddHeader("Command Count");
            this.AddHeader("Category");

        }
        
        public void AddHeader(string header)
        {
            this.Columns.Add(header);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (this.selectionHandler != null && this.SelectedItems.Count > 0)
            {
                var selectedItem = this.SelectedItems[0] as AreaListViewItem;
                if (selectedItem != null)
                {
                    this.selectionHandler(selectedItem.Command);
                }
            }   
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

        public void Populate(CubaseCommandsCollection cubaseCommands, CubaseServerSettings cubaseServerSettings, Action<CubaseCommandCollection>? selectionHandler = null)
        {
            if (selectionHandler != null)
            {
                this.selectionHandler = selectionHandler;
            }
            this.cubaseCommands = cubaseCommands;
            this.cubaseServerSettings = cubaseServerSettings;   
            this.ContextMenuStrip = new AreaContextMenuStrip(cubaseCommands, cubaseServerSettings, this);
            this.Items.Clear();
            foreach (var cmd in cubaseCommands)
            {
                this.Items.Add(new AreaListViewItem(cmd));
            }
            this.AutoFit();
        }

        public void RefreshCommands()
        {
            this.Populate(this.cubaseCommands, this.cubaseServerSettings);  
        }
    }

    public class AreaListViewItem : ListViewItem
    {
        public CubaseCommandCollection Command { get; set; }

        public AreaListViewItem(CubaseCommandCollection cubaseCommandCollection) : base()
        {
            this.Command = cubaseCommandCollection;
            this.Text = this.Command.Name;
            this.SubItems.Add(this.Command.Visible ? "Yes" : "No");
            this.SubItems.Add(this.Command.Commands.Count.ToString()); 
            // this.SubItems.Add(this.Command.Category);   
        }
    }
}
