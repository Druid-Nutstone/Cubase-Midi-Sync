using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using Cubase.Midi.Sync.Configuration.UI.Controls.Macros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Midi
{
    public class MidiCommandListView : ListView
    {
        private Action<CubaseMidiCommand> Handler;

        public MidiCommandListView() : base() 
        {
            this.View = View.Details;
            this.MultiSelect = false;
            this.FullRowSelect = true;
            this.Dock = DockStyle.Fill; 
            this.AddHeader("Name");
            this.AddHeader("Command");
        }

        public void AddHeader(string text)
        {
            this.Columns.Add(text);
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

        protected override void OnDoubleClick(EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                var selectedItem = this.SelectedItems[0] as MidiCommandListViewItem;
                if (selectedItem != null)
                {
                    this.Handler(selectedItem.Command);
                }
            }
        }

        public void Populate(List<CubaseMidiCommand> commands, Action<CubaseMidiCommand> handler)
        {
            this.Handler = handler;
            this.Items.Clear();
            foreach (CubaseMidiCommand command in commands)
            {
                this.Items.Add(new MidiCommandListViewItem(command));   
            }
            this.AutoFit();
        }

    }

    public class MidiCommandListViewItem : ListViewItem
    {
        public CubaseMidiCommand Command;
        
        public MidiCommandListViewItem(CubaseMidiCommand cubaseMidiCommand) 
        { 
            this.Command = cubaseMidiCommand;
            this.Text = this.Command.Name;
            this.SubItems.Add(this.Command.Command);
        }
    }
}
