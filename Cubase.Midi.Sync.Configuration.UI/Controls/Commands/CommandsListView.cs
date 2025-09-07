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

        public string areaFilter;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<CubaseCommand> OnCommandSelected {  get; set; }   

        public CommandsListView() : base()
        {
            this.View = View.Details;
            this.DoubleBuffered = true;
            this.FullRowSelect = true;
            this.MultiSelect = false;
            this.AddHeader("Area");
            this.AddHeader("Button Name");
            this.AddHeader("Action");
            this.AddHeader("Toggle Button");
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

        public void PopulateSubset(CubaseCommandCollection commandSubset)
        {
            this.Items.Clear();
            commandSubset.Commands.ForEach(cubaseCommand =>
            {
                if (CanAddCommand(cubaseCommand))
                {
                    this.Items.Add(new CommandsListViewItem(cubaseCommand.ParentCollectionName, cubaseCommand));
                }
            });
            this.AutoFit();
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
                    if (CanAddCommand(cubaseCommand))
                    {
                        this.Items.Add(new CommandsListViewItem(command.Name, cubaseCommand));
                    }
                });
            });
            this.AutoFit();
            this.ContextMenuStrip = null;
            this.ContextMenuStrip = new CommandsContextMenuStrip(commands, cubaseServerSettings, this);
        }

        private bool CanAddCommand(CubaseCommand command)
        {
            return this.areaFilter == null ? true : command.ParentCollectionName == this.areaFilter;    
        } 

        public bool HaveSelectedCubaseCommd => this.SelectedItems.Count > 0;

        public CubaseCommand GetSelectedCubaseCommand()
        {
            if (this.SelectedItems.Count > 0)
            {
                return ((CommandsListViewItem)this.SelectedItems[0]).Command;
            }
            return null;
        }

        public void SetAreaFilter(string areName)
        {
            this.areaFilter = areName;  
            this.Populate(commands, cubaseServerSettings);
        }

        public void RefreshCubaseCommands()
        {
            this.commands = cubaseServerSettings.GetCubaseCommands();
            this.Populate(this.commands, this.cubaseServerSettings);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                var cubaseItem = (CommandsListViewItem)this.SelectedItems[0];
                OnCommandSelected?.Invoke(cubaseItem.Command);
            }
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
            this.SubItems.Add(string.IsNullOrEmpty(cmd.Action) ? "MACRO" : cmd.Action);
            this.SubItems.Add(cmd.IsToggleButton ? "Yes" : "No");
            this.SetColours();
        }

        private void SetColours()
        {
            if (this.Command.IsMacro)
            {
                this.ForeColor = Color.Blue;
            }
        }
    }  
}
