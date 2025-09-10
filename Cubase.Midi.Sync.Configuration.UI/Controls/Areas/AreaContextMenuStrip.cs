using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Configuration.UI.Controls.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Areas
{
    public class AreaContextMenuStrip : ContextMenuStrip 
    {
        public AreaContextMenuStrip(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, AreaListView listView)
        {
            this.Items.Add(new MoveCommandUpMenu(commands,cubaseServerSettings, listView));
            this.Items.Add(new MoveCommandDownMenu(commands, cubaseServerSettings, listView));
            this.Items.Add(new VisibleCommandMenu(commands, cubaseServerSettings, listView));
            this.Items.Add(new HideCommandMenu(commands, cubaseServerSettings, listView));
            this.Items.Add(new DeleteCommandMenu(commands, cubaseServerSettings, listView));
            this.Items.Add(new CategoryMenu(commands, cubaseServerSettings, listView));
        }
    }

    public class CategoryMenu : ToolStripMenuItem
    {
        public CubaseCommandsCollection commands;

        public CubaseServerSettings cubaseServerSettings;

        public AreaListView listView;

        public CategoryMenu(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, AreaListView listView)
        {
            this.commands = commands;
            this.cubaseServerSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Category";
            this.DropDownItems.Add(new CategoryMenuKeys(this.commands, this.cubaseServerSettings, this.listView));
            this.DropDownItems.Add(new CategoryMenuMidi(this.commands, this.cubaseServerSettings, this.listView));
        }
    }

    public class CategoryMenuKeys : ToolStripMenuItem
    {
        public CubaseCommandsCollection commands;

        public CubaseServerSettings cubaseServerSettings;

        public AreaListView listView;

        public CategoryMenuKeys(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, AreaListView listView)
        {
            this.commands = commands;
            this.cubaseServerSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Keys";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubase = (AreaListViewItem)this.listView.SelectedItems[0];
            cubase.Command.Category = CubaseAreaTypes.Keys.ToString();  
            this.commands.SaveToFile(this.cubaseServerSettings.FilePath);
            listView.RefreshCommands();
        }
    }

    public class CategoryMenuMidi : ToolStripMenuItem
    {
        public CubaseCommandsCollection commands;

        public CubaseServerSettings cubaseServerSettings;

        public AreaListView listView;

        public CategoryMenuMidi(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, AreaListView listView)
        {
            this.commands = commands;
            this.cubaseServerSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Midi";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubase = (AreaListViewItem)this.listView.SelectedItems[0];
            cubase.Command.Category = CubaseAreaTypes.Midi.ToString();
            this.commands.SaveToFile(this.cubaseServerSettings.FilePath);
            listView.RefreshCommands();
        }   
    }

    public class MoveCommandUpMenu : ToolStripMenuItem
    {
        public CubaseCommandsCollection commands;

        public CubaseServerSettings cubaseServerSettings;

        public AreaListView listView;
        
        public MoveCommandUpMenu(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, AreaListView listView)
        {
            this.commands = commands;
            this.cubaseServerSettings = cubaseServerSettings;
            this.listView = listView;   
            this.Text = "Move Up";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubase = (AreaListViewItem)this.listView.SelectedItems[0];
            var commandIndex = this.commands.FindIndex(x => x.Name == cubase.Command.Name);
            if (commandIndex > 0)
            {
                var tmp = this.commands[commandIndex];
                this.commands[commandIndex] = this.commands[commandIndex - 1];
                this.commands[commandIndex - 1] = tmp;
            }
            this.commands.SaveToFile(this.cubaseServerSettings.FilePath);
            listView.RefreshCommands();
        }
    }

    public class MoveCommandDownMenu : ToolStripMenuItem
    {
        public CubaseCommandsCollection commands;

        public CubaseServerSettings cubaseServerSettings;

        public AreaListView listView;

        public MoveCommandDownMenu(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, AreaListView listView)
        {
            this.commands = commands;
            this.cubaseServerSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Move Down";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubase = (AreaListViewItem)this.listView.SelectedItems[0];
            var commandIndex = this.commands.FindIndex(x => x.Name == cubase.Command.Name);
            if (commandIndex > 0)
            {
                var tmp = this.commands[commandIndex];
                this.commands[commandIndex] = this.commands[commandIndex + 1];
                this.commands[commandIndex + 1] = tmp;
            }
            this.commands.SaveToFile(this.cubaseServerSettings.FilePath);
            listView.RefreshCommands();
        }
    }

    public class DeleteCommandMenu : ToolStripMenuItem
    {
        public CubaseCommandsCollection commands;

        public CubaseServerSettings cubaseServerSettings;

        public AreaListView listView;

        public DeleteCommandMenu(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, AreaListView listView)
        {
            this.commands = commands;
            this.cubaseServerSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Remove";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubase = (AreaListViewItem)this.listView.SelectedItems[0];
            var commandIndex = this.commands.FindIndex(x => x.Name == cubase.Command.Name);
            this.commands.RemoveAt(commandIndex);
            this.commands.SaveToFile(this.cubaseServerSettings.FilePath);
            listView.RefreshCommands();
        }
    }

    public class VisibleCommandMenu : ToolStripMenuItem
    {
        public CubaseCommandsCollection commands;

        public CubaseServerSettings cubaseServerSettings;

        public AreaListView listView;

        public VisibleCommandMenu(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, AreaListView listView)
        {
            this.commands = commands;
            this.cubaseServerSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Visible To Client";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubase = (AreaListViewItem)this.listView.SelectedItems[0];
            cubase.Command.Visible = true;
            this.commands.SaveToFile(this.cubaseServerSettings.FilePath);
            listView.RefreshCommands();
        }
    }

    public class HideCommandMenu : ToolStripMenuItem
    {
        public CubaseCommandsCollection commands;

        public CubaseServerSettings cubaseServerSettings;

        public AreaListView listView;

        public HideCommandMenu(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, AreaListView listView)
        {
            this.commands = commands;
            this.cubaseServerSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "NOT visible to client";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubase = (AreaListViewItem)this.listView.SelectedItems[0];
            cubase.Command.Visible = false;
            this.commands.SaveToFile(this.cubaseServerSettings.FilePath);
            listView.RefreshCommands();
        }
    }
}
