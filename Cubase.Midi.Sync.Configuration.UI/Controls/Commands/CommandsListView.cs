using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Commands
{
    public class CommandsListView : ListView
    {
        private CubaseCommandsCollection commands;

        private CubaseServerSettings cubaseServerSettings;

        public CommandsListView() : base()
        {
            this.View = View.Details;
            this.DoubleBuffered = true;
            this.FullRowSelect = true;
            this.MultiSelect = false;
            this.AddHeader("Category");
            this.AddHeader("Button Name");
            this.AddHeader("Action");
            this.AddHeader("Toggle Button");
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

        public void Populate(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings)
        {
            this.Items.Clear(); 
            this.commands = commands;
            this.cubaseServerSettings = cubaseServerSettings;   
            this.commands.ForEach(command => 
            {
                command.Commands.ForEach(cubaseCommand => 
                { 
                    this.Items.Add(new CommandsListViewItem(command.Name, cubaseCommand)); 
                });
            });
            this.AutoFit();
            this.ContextMenuStrip = null;
            this.ContextMenuStrip = new CommandsContextMenuStrip(commands, cubaseServerSettings, this);
        }
    }

    public class CommandsListViewItem : ListViewItem
    {
        public string Category { get; set; }    
        
        public CubaseCommand Command { get; set; }  
        
        public CommandsListViewItem(string category, CubaseCommand cmd) 
        { 
            this.Text = category;
            this.Command = cmd;
            this.Category = category;   
            this.SubItems.Add(cmd.Name);
            this.SubItems.Add(cmd.Action);
            this.SubItems.Add(cmd.ButtonType == CubaseButtonType.Toggle ? "Yes" : "No");
        }
    }  
}
