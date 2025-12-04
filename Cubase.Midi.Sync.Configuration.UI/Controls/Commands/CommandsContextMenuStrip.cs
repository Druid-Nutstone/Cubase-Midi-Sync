using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using Cubase.Midi.Sync.Configuration.UI.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Commands
{
    public class CommandsContextMenuStrip : ContextMenuStrip
    {
        public CommandsContextMenuStrip(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, CommandsListView listView)
        {
            this.Items.Clear();
            this.Items.Add(new EditCommand(commands, cubaseServerSettings, listView));
            this.Items.Add(new RunCommand(commands, cubaseServerSettings, listView));
            this.Items.Add(new MoveUpCommand(commands, cubaseServerSettings, listView));
            this.Items.Add(new MoveDownCommand(commands, cubaseServerSettings, listView));
            this.Items.Add(new DeleteSingleCommand(commands, cubaseServerSettings, listView));
            this.Items.Add(new DeleteCommand(commands, cubaseServerSettings, listView));
        }
    }


    public class DeleteCommand : ToolStripMenuItem
    {
        private CubaseCommandsCollection commands;

        private CubaseServerSettings serverSettings;

        private CommandsListView listView;

        public DeleteCommand(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, CommandsListView listView)
        {
            this.commands = commands;
            this.serverSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Delete All Occurences Of This Command";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubaseCommandItem = (CommandsListViewItem)this.listView.SelectedItems[0];
            this.commands.RemoveCubaseCommand(cubaseCommandItem.Command, true);
            commands.SaveToFile(serverSettings.FilePath);
            listView.RefreshCubaseCommands();
        }
    }

    public class MoveUpCommand : ToolStripMenuItem
    {
        private CubaseCommandsCollection commands;

        private CubaseServerSettings serverSettings;

        private CommandsListView listView;

        public MoveUpCommand(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, CommandsListView listView)
        {
            this.commands = commands;
            this.serverSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Move Up";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubaseCommandItem = (CommandsListViewItem)this.listView.SelectedItems[0];
            // get the associated [arent 
            var parent = this.commands.GetCommandCollectionByName(cubaseCommandItem.Command.ParentCollectionName);
            var commandIndex = parent.Commands.FindIndex(x => x.Name == cubaseCommandItem.Command.Name);
            if (commandIndex > 0)
            {
                var tmp = parent.Commands[commandIndex];
                parent.Commands[commandIndex] = parent.Commands[commandIndex - 1];
                parent.Commands[commandIndex - 1] = tmp;
            }
            this.commands.SaveToFile(this.serverSettings.FilePath);
            listView.RefreshCubaseCommands();
        }
    }

    public class MoveDownCommand : ToolStripMenuItem
    {
        private CubaseCommandsCollection commands;

        private CubaseServerSettings serverSettings;

        private CommandsListView listView;

        public MoveDownCommand(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, CommandsListView listView)
        {
            this.commands = commands;
            this.serverSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Move Down";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubaseCommandItem = (CommandsListViewItem)this.listView.SelectedItems[0];
            // get the associated [arent 
            var parent = this.commands.GetCommandCollectionByName(cubaseCommandItem.Command.ParentCollectionName);
            var commandIndex = parent.Commands.FindIndex(x => x.Name == cubaseCommandItem.Command.Name);
            if (commandIndex >= 0 && commandIndex < parent.Commands.Count - 1) // ensure not last item
            {
                // swap with the one below
                var tmp = parent.Commands[commandIndex];
                parent.Commands[commandIndex] = parent.Commands[commandIndex + 1];
                parent.Commands[commandIndex + 1] = tmp;
            }
            this.commands.SaveToFile(this.serverSettings.FilePath);
            listView.RefreshCubaseCommands();
        }
    }

    public class DeleteSingleCommand : ToolStripMenuItem
    {
        private CubaseCommandsCollection commands;

        private CubaseServerSettings serverSettings;

        private CommandsListView listView;

        public DeleteSingleCommand(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, CommandsListView listView)
        {
            this.commands = commands;
            this.serverSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Delete This Command";
        }

        protected override void OnClick(EventArgs e)
        {
            var cubaseCommandItem = (CommandsListViewItem)this.listView.SelectedItems[0];
            this.commands.RemoveCubaseCommand(cubaseCommandItem.Command);
            commands.SaveToFile(serverSettings.FilePath);
            listView.RefreshCubaseCommands();
        }
    }

    public class RunCommand : ToolStripMenuItem
    {
        private CubaseCommandsCollection commands;

        private CubaseServerSettings serverSettings;

        private CommandsListView listView;

        public RunCommand(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, CommandsListView listView)
        {
            this.commands = commands;
            this.serverSettings = cubaseServerSettings;
            this.listView = listView;
            this.Text = "Run";
        }

        protected override void OnClick(EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0)
            {
                var cubaseCommandItem = (CommandsListViewItem)this.listView.SelectedItems[0];
                var command = cubaseCommandItem.Command;
                var cubaseSocketRequest = CubaseActionRequest.CreateFromCommand(command, command.ActionGroup);
                var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction, cubaseSocketRequest);
                Task.Run(async () =>
                {
                    await CubaseWebSocketClient.Instance.SendCommand(socketMessage);
                });
            }
        }
    }

    public class EditCommand: ToolStripMenuItem
    {
        private CubaseCommandsCollection commands;

        private CubaseServerSettings serverSettings;

        private CommandsListView listView;
        
        public EditCommand(CubaseCommandsCollection commands, CubaseServerSettings cubaseServerSettings, CommandsListView listView)
        {
            this.commands = commands;
            this.serverSettings = cubaseServerSettings; 
            this.listView = listView;
            this.Text = "Edit";

        }

        protected override void OnClick(EventArgs e)
        {
            if (this.listView.SelectedItems.Count > 0) 
            {
                var cubaseCommandItem  = (CommandsListViewItem)this.listView.SelectedItems[0];
                var keyCommandForm = new AddKeyToCommandsForm(cubaseCommandItem.Command, commands, serverSettings, cubaseCommandItem.Category);
                keyCommandForm.ShowDialog();
                listView.RefreshCubaseCommands();
            }
        }
    }
}
