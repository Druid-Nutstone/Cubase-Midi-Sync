using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    public class KeysContentMenuStrip : ContextMenuStrip
    {
        private readonly CubaseCommandsCollection cubaseCommandCollections;

        public KeysContentMenuStrip(CubaseCommandsCollection cubaseCommandCollections, KeysListView keysListView, CubaseServerSettings cubaseServerSettings)
        {
            this.cubaseCommandCollections = cubaseCommandCollections;
            this.Items.Clear();
            this.Items.Add(new AddKeyMenutdem(this.cubaseCommandCollections, keysListView, cubaseServerSettings));

        }
    }

    public class AddKeyMenutdem : ToolStripMenuItem
    {
        private readonly CubaseCommandsCollection cubaseCommandCollections;

        private readonly KeysListView keysListView;

        private readonly CubaseServerSettings cubaseServerSettings;

        public AddKeyMenutdem(CubaseCommandsCollection cubaseCommandCollections, KeysListView keysListView, CubaseServerSettings cubaseServerSettings)
        {
            this.cubaseCommandCollections = cubaseCommandCollections;   
            this.keysListView = keysListView;
            this.cubaseServerSettings = cubaseServerSettings;   
            this.Text = "Add Key";  
        }

        protected override void OnClick(EventArgs e)
        {
            if (keysListView.SelectedItems.Count > 0)
            {
                foreach (var selectedItem in keysListView.SelectedItems)
                {
                    if (selectedItem is KeysListViewItem currentItem)
                    {
                        var command = currentItem.Command;
                        var commandExists =  this.cubaseCommandCollections.SelectMany(x => x.Commands)
                                                                           .Any(x => x.Action?.Action?.Trim() == command.Action.Action.Trim());   
                        if (!commandExists)
                        {
                            var addForm = new AddKeyToCommandsForm(currentItem.Command, this.cubaseCommandCollections, this.cubaseServerSettings);
                            addForm.ShowDialog();
                        }
                    }
                }
            }
        }
    }
}
