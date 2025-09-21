using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Keys;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.ImportExport
{
    public partial class ImportExportControl : UserControl
    {
        private CubaseServerSettings cubaseServerSettings;

        private CubaseCommandsCollection cubaseCommands;

        private CubaseCommandsCollection newCubaseCommands;

        private CubaseKeyCommandCollection cubaseKeyCommands;

        private CubaseCommandsCollection importedCubaseCommands;

        public ImportExportControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            ExportButton.Visible = false;
            ImportButton.Visible = false;
            ExportButton.Click += ExportButton_Click;
            ImportButton.Click += ImportButton_Click;
            FilterDate.Visible = false;
            filterDateLabel.Visible = false;    
            this.LoadLocalCubaseCommands();
            this.FilterDate.ValueChanged += FilterDate_ValueChanged;
            
        }

        private void FilterDate_ValueChanged(object? sender, EventArgs e)
        {
            var dateSpecifiedCommands = this.cubaseCommands.SelectMany(x => x.Commands)
                                                           .Where(x => x.Created != null)
                                                           .Where(x => DateTime.Compare(x.Created.GetValueOrDefault().Date, FilterDate.Value.Date) >= 0)
                                                           .ToList();
            this.importExportListView.Populate(dateSpecifiedCommands);
        }

        private void ImportButton_Click(object? sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json|*.json";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.importedCubaseCommands = CubaseCommandsCollection.LoadFromFile(openFileDialog.FileName);
                this.importExportListView.Populate(this.importedCubaseCommands.SelectMany(x => x.Commands).ToList());
                this.ImportButton.Enabled = false; 
            }
        }

        private void ExportButton_Click(object? sender, EventArgs e)
        {
            if (this.newCubaseCommands != null)
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Json|*.json";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.newCubaseCommands.SaveToFile(saveFileDialog.FileName); 
                }
                MessageBox.Show($"Cubase commands saved to {saveFileDialog.FileName}");
            }
        }

        public void Export()
        {
            ExportButton.Visible = true;
            ImportButton.Visible = false;
            FilterDate.Visible = true;
            filterDateLabel.Visible = true;
            this.importExportListView.Populate(this.cubaseCommands.SelectMany(x => x.Commands).ToList());
            this.newCubaseCommands = new CubaseCommandsCollection();
            this.importExportListView.ItemCheckSelected = (command, check) => 
            {
                if (check)
                {
                    if (newCubaseCommands.HaveName(command.ParentCollectionName))
                    {
                        var collection = newCubaseCommands.GetCommandCollectionByName(command.ParentCollectionName);
                        collection.Commands.Add(command);
                    }
                    else
                    {
                        var collection = newCubaseCommands.WithNewCubaseCommand(command.ParentCollectionName, command.Category);
                        collection.Commands.Add(command);
                    }
                }
            };
        }

        public void Import()
        {
            this.LoadLocalCubaseCommands();
            ExportButton.Visible = false;
            ImportButton.Enabled = true;
            ImportButton.Visible = true;    
            this.cubaseKeyCommands = new CubaseKeyCommandParser().Parse(CubaseConfigurationConstants.KeyCommandsFileLocation);
            this.importExportListView.ItemCheckSelected = (command, selected) => 
            {
                if (selected)
                {
                    CubaseCommandCollection commandCollection;
                    // if the collection name already exists 
                    if (this.cubaseCommands.HaveName(command.ParentCollectionName))
                    {
                        commandCollection = this.cubaseCommands.GetCommandCollectionByName(command.ParentCollectionName);
                        var commandExists = commandCollection.Commands.Any(x => x.Name == command.Name && x.ButtonType == command.ButtonType);
                        if (commandExists)
                        {
                            MessageBox.Show($"Command {command.Name} {command.ButtonType} already exists in {command.ParentCollectionName}");
                            return;
                        }
                    }
                    else // create new collection 
                    {
                        commandCollection = this.cubaseCommands.WithNewCubaseCommand(command.ParentCollectionName, command.Category);
                    }
                    if (command.ButtonType == CubaseButtonType.Momentory || command.ButtonType == CubaseButtonType.Toggle)
                        // ensure the command exists in cubase 
                        if (!this.cubaseKeyCommands.IsInCubase(command.CubaseCommandDefinition, command.Action))
                        {
                            var copyForm = new AllowCopyCubaseKeyCommandForm(command.CubaseCommandDefinition.CommandDescription, command.Action);
                            copyForm.ShowDialog();
                            // MessageBox.Show($"You need to add this command to cubase. Find {command.CubaseCommandDefinition.CommandDescription} and assign it to {command.Action}. Then press OK to continue");
                        }
                    commandCollection.Commands.Add(command);
                    // and save it for good measure 
                    this.cubaseCommands.SaveToFile(this.cubaseServerSettings.FilePath);
                }
            };
        }

        private void LoadLocalCubaseCommands()
        {
            this.cubaseServerSettings = new CubaseServerSettings();
            this.cubaseCommands = cubaseServerSettings.GetCubaseCommands();
        }
    }
}
