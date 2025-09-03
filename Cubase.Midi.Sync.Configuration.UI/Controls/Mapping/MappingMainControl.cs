using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Colours;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Mapping
{
    public partial class MappingMainControl : UserControl
    {
        private CubaseCommandsCollection commands;
        
        private CubaseServerSettings cubaseServerSettings;

        private CubaseCommandCollection currentCommand;

        public MappingMainControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.CreateButton.Click += CreateButton_Click;
            this.NewAreaName.KeyPress += NewAreaName_KeyPress;
            this.NewAreaName.LostFocus += NewAreaName_LostFocus;
            this.ButtonCopy.Click += ButtonCopy_Click;
            this.ExistingArea.SelectedIndexChanged += ExistingArea_SelectedIndexChanged;
            this.AreaBackgroundColour.ColourChanged = this.AreaBackGroundColourChanged;
            this.AreaTextColour.ColourChanged = this.AreaTextColourChanged;
        }

        private void AreaBackGroundColourChanged(SerializableColour colour)
        {
            if (this.currentCommand != null)
            {
                this.currentCommand.BackgroundColour = colour;  
            }
        }

        private void AreaTextColourChanged(SerializableColour colour)
        {
            if (this.currentCommand != null)
            {
                this.currentCommand.TextColour = colour;    
            }
        }

        private void ExistingArea_SelectedIndexChanged(object? sender, EventArgs e)
        {
            this.currentCommand = this.commands.FirstOrDefault(x => x.Name == ExistingArea.SelectedItem.ToString());
            // NewAreaName.Enabled = false;
            this.AreaBackgroundColour.SetColour(this.currentCommand.BackgroundColour);
            this.AreaTextColour.SetColour(this.currentCommand.TextColour);
            this.mappingListView.Items.Clear();
            foreach (var command in this.currentCommand.Commands)
            {
                this.mappingListView.PopulateCubaseCommand(command);   
            }
        }

        private void ButtonCopy_Click(object? sender, EventArgs e)
        {
            if (this.commandsListView.HaveSelectedCubaseCommd)
            {
                this.CubaseCommandCopy(this.commandsListView.GetSelectedCubaseCommand());
            }
        }

        private void CubaseCommandCopy(CubaseCommand command)
        {
            if (this.currentCommand != null)
            {
                if (!this.currentCommand.Commands.Any(x => x.Name == command.Name))
                {
                    var newCommand = CubaseCommand.Create()
                                                  .WithButtonType(command.ButtonType)
                                                  .WithName(command.Name)
                                                  .WithCategory(this.NewAreaName.Text)
                                                  .WithAction(command.Action)  
                                                  .WithNameToggle(command.NameToggle)
                                                  .WithParentCollectionName(this.currentCommand.Name)   
                                                  .WithActionGroupToggleOff(command.ActionGroupToggleOff)   
                                                  .WithActionGroup(command.ActionGroup);
                    newCommand.ButtonBackgroundColour = command.ButtonBackgroundColour;
                    newCommand.ButtonTextColour = command.ButtonTextColour;
                    newCommand.ToggleBackGroundColour = command.ToggleBackGroundColour;
                    newCommand.ToggleForeColour = command.ToggleForeColour;
                    this.currentCommand.WithNewCubaseCommand(newCommand);
                    this.commands.SaveToFile(this.cubaseServerSettings.FilePath);
                    this.PopulateCommandsListView();
                    this.mappingListView.PopulateCubaseCommand(newCommand);
                }
                else
                {
                    MessageBox.Show("The item already exists");
                }
            }
            else
            {
                MessageBox.Show("Creating a new Mapping (Category) button first!");
            }
        }

        private void NewAreaName_LostFocus(object? sender, EventArgs e)
        {
            this.CreateNewArea();
        }

        private void NewAreaName_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
            {
                this.CreateNewArea();
            }
        }

        public void CreateNewArea()
        {
            if (!string.IsNullOrEmpty(this.NewAreaName.Text))
            {
                if (!this.commands.HaveName(this.NewAreaName.Text))
                {
                    this.currentCommand = this.commands.WithNewCubaseCommand(this.NewAreaName.Text, CubaseServiceConstants.KeyService);
                    this.AreaBackgroundColour.SetColour(this.currentCommand.BackgroundColour);
                    this.AreaTextColour.SetColour(this.currentCommand.TextColour);
                    this.mappingListView.Items.Clear();
                }
            }
        }

        private void CreateButton_Click(object? sender, EventArgs e)
        {
            this.CreateNewArea();
        }

        public void Populate()
        {
            this.cubaseServerSettings = new CubaseServerSettings();
            this.commands = cubaseServerSettings.GetCubaseCommands();
            this.mappingListView.Items.Clear();
            this.PopulateCommandsListView();
            this.PopulateExistingAreas();
        }

        private void PopulateExistingAreas()
        {
            this.ExistingArea.Items.Clear();
            this.ExistingArea.Items.AddRange(this.commands.GetNames().ToArray());
        }

        private void PopulateCommandsListView()
        {
            this.commandsListView.Populate(this.commands, cubaseServerSettings);
            Resize();
        }

        public void Resize()
        {
            LeftPanel.Width = this.Width * 40 / 100;
            ButtonCopy.Left = (ButtonPanel.Width - ButtonCopy.Width) / 2; // horizontal center
            ButtonCopy.Top = (ButtonPanel.Height - ButtonCopy.Height) / 2;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Resize();
        }
    }
}
