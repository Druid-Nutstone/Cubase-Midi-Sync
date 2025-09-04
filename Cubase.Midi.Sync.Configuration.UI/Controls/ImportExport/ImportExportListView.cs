using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.ImportExport
{
    public class ImportExportListView : ListView
    {

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<CubaseCommand, bool> ItemCheckSelected { get; set; }  

       
        public ImportExportListView() : base() 
        {
            this.CheckBoxes = true;
            this.View = View.Details;
            this.AddHeader("Area");
            this.AddHeader("Name");
            this.AddHeader("Button Type");
            this.AddHeader("Command");
            this.AddHeader("Cubase Area");
            
        }

        protected override void OnItemChecked(ItemCheckedEventArgs e)
        {
            var cubaseCommand = ((ImportExortListViewItem)e.Item).Command;
            if (this.ItemCheckSelected != null)
            {
                this.ItemCheckSelected(cubaseCommand, e.Item.Checked);
            }
        }



        public void AddHeader(string header)
        {
            this.Columns.Add(header);
        }

        public void Populate(List<CubaseCommand> cubaseCommands)
        {
            this.Items.Clear();
            foreach (var command in cubaseCommands) 
            {
                this.Items.Add(new ImportExortListViewItem(command));
            }
            this.AutoFit();
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




    }

    public class ImportExortListViewItem : ListViewItem 
    {
        public CubaseCommand Command { get; set; }
        
        public ImportExortListViewItem(CubaseCommand buttonCommand)
        {
            this.Command = buttonCommand;
            this.Text = this.Command.ParentCollectionName;
            this.SubItems.Add(this.Command.Name);
            this.SubItems.Add(this.Command.ButtonType.ToString());
            var action = this.Command.Action;
            switch (this.Command.ButtonType)
            {
                case CubaseButtonType.Macro:
                    action = $"Macro";
                    break;
                case CubaseButtonType.MacroToggle:
                    action = $"MacroToggle";
                    break;
            }
            this.SubItems.Add(action);
            this.SubItems.Add(this.Command.CubaseCommandDefinition?.CommandDescription);
        }
    }
}
 