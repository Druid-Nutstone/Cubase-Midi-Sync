using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Configuration.UI.Controls.Commands;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiMacros.Forms
{
    public class MidiMacroSelectListView : ListView 
    {
        private Action<CubaseMacroCommand> macroSelectedHandler;
        
        public MidiMacroSelectListView() : base() 
        {
            this.View = View.Details;
            this.MultiSelect = false;
            this.FullRowSelect = true;
            this.AddHeader("Macro Name");
        }

        public void AddHeader(string header)
        {
            this.Columns.Add(header);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                var cubaseMacro = (MidiMacroSelectListViewItem)this.SelectedItems[0];
                macroSelectedHandler?.Invoke(cubaseMacro.Macro);
            }
        }

        public void Populate(CubaseMacroCommandCollection commands, Action<CubaseMacroCommand> macroSelected)
        {
            this.macroSelectedHandler = macroSelected;
            this.Items.Clear();
            foreach (var command in commands)
            {
                this.Items.Add(new MidiMacroSelectListViewItem(command));
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

    public class MidiMacroSelectListViewItem : ListViewItem
    {
        public CubaseMacroCommand Macro { get; set; }
        
        public MidiMacroSelectListViewItem(CubaseMacroCommand cubaseMacro)
        {
            this.Macro = cubaseMacro;
            this.Text = cubaseMacro.Name;
        }
    }
}
