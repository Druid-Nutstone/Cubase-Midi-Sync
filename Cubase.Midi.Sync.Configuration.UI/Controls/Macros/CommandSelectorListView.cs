using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Macros
{
    public class CommandSelectorListView : ListView
    {
        private Action<CubaseKeyCommand> Handler;

        public CommandSelectorListView() : base()
        {
            this.View = View.Details;
            this.MultiSelect = false;
            this.DoubleBuffered = true;
            this.AddHeader("Category");
            this.AddHeader("Name");
            this.AddHeader("Key");
            this.AddHeader("Cubase Description");
            this.FullRowSelect = true;
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
                var selectedItem = this.SelectedItems[0] as CommandSelectorListVieItem;
                if (selectedItem != null)
                {
                    this.Handler(selectedItem.Command);
                }
            }
        }

        public void Populate(List<CubaseKeyCommand> commands, Action<CubaseKeyCommand> handler)
        {
            this.Items.Clear();
            this.Handler = handler;
            commands.ForEach(command => 
            {
                this.Items.Add(new CommandSelectorListVieItem(command));
            });
            this.AutoFit();
        }
    }

    public class CommandSelectorListVieItem : ListViewItem
    {
        public CubaseKeyCommand Command { get; set; }


        public CommandSelectorListVieItem(CubaseKeyCommand cubaseKeyCommand)
        {
            this.Command = cubaseKeyCommand;
            this.Text = cubaseKeyCommand.Category;
            this.SubItems.Add(cubaseKeyCommand.Name);
            this.SubItems.Add(cubaseKeyCommand.Key);
            this.SubItems.Add(cubaseKeyCommand.CubaseCommand?.CommandDescription);
        }
    }
}
