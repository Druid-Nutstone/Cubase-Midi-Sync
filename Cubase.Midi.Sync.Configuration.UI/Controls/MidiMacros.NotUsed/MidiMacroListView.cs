using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiMacros
{
    public class MidiMacroListView : ListView
    {
        public MidiMacroListView() : base() 
        {
            this.View = View.Details;
            this.Dock = DockStyle.Fill; 
            this.FullRowSelect = true;
            this.MultiSelect = false;
            this.AddHeader("Name");
            this.AddHeader("Channel");
            this.AddHeader("Note");
        }

        public void AddHeader(string header)
        {
            this.Columns.Add(header);
        }

        public void Populate(CubaseMacroCollection cubaseMacros)
        {
            this.Items.Clear();
            foreach (var macro in cubaseMacros) 
            { 
                this.Items.Add(new MidiMacroListViewItem(macro));   
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

    public class MidiMacroListViewItem : ListViewItem 
    { 
        public CubaseMacro Macro { get; set; }  
        
        public MidiMacroListViewItem(CubaseMacro cubaseMacro)
        {
            this.Macro = cubaseMacro;
            this.Text = cubaseMacro.Name;
            this.SubItems.Add(cubaseMacro.Channel.ToString());
            this.SubItems.Add(cubaseMacro.Note.ToString()); 
        }
    }
}

