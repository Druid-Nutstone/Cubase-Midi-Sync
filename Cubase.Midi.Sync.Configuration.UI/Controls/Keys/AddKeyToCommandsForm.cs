using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Extensions;
using Cubase.Midi.Sync.Configuration.UI.Controls.Macros;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    public partial class AddKeyToCommandsForm : Form
    {
        private CubaseKeyCommand cubaseKeyCommand;

        private CubaseCommandsCollection cubaseCommandCollections;

        private CubaseServerSettings cubaseServerSettings;

        private CubaseCommand cubaseCommand;

        public AddKeyToCommandsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add a NEW command for cubase 
        /// </summary>
        /// <param name="cubaseKeyCommand"></param>
        /// <param name="cubaseCommandCollections"></param>
        /// <param name="cubaseServerSettings"></param>
        public AddKeyToCommandsForm(CubaseKeyCommand cubaseKeyCommand, CubaseCommandsCollection cubaseCommandCollections, CubaseServerSettings cubaseServerSettings)
        {
            InitializeComponent();
            this.cubaseCommand = CubaseCommand.Create();
            this.cubaseCommandCollections = cubaseCommandCollections;
            this.InitialiseControls(cubaseKeyCommand, cubaseCommandCollections, cubaseServerSettings);
        }

        /// <summary>
        /// Update an existing command 
        /// </summary>
        /// <param name="existingCommand"></param>
        /// <param name="cubaseKeyCommand"></param>
        /// <param name="cubaseCommandCollections"></param>
        /// <param name="cubaseServerSettings"></param>
        public AddKeyToCommandsForm(CubaseCommand existingCommand, CubaseCommandsCollection cubaseCommandCollections, CubaseServerSettings cubaseServerSettings, string category)
        {
            InitializeComponent();
            this.cubaseCommand = existingCommand;
            this.cubaseCommandCollections = cubaseCommandCollections;
            this.InitialiseControls(cubaseKeyCommand, cubaseCommandCollections, cubaseServerSettings);
            this.cbAreaName.SelectedIndex = cubaseCommandCollections.FindIndex(x => x.Name == category);
            this.cbAreaName.Enabled = false;
            ButtonTextColour.SetColour(existingCommand.TextColor);
            ButtonToggleBackgroundColour.SetColour(existingCommand.ToggleBackGroundColour);
            ButtonBackgroundColour.SetColour(existingCommand.ButtonBackgroundColour);
            ButtonToggleTextColour.SetColour(existingCommand.ToggleForeColour);  
            this.buttonAdd.Text = "Update";
        }

        private void InitialiseControls(CubaseKeyCommand cubaseKeyCommand, CubaseCommandsCollection cubaseCommandCollections, CubaseServerSettings cubaseServerSettings)
        {
            this.buttonCubaseCommands.Visible = false;
            this.buttonCubaseCommands.Click += ButtonCubaseCommands_Click;
            this.cubaseCommandCollections = cubaseCommandCollections;
            this.cubaseKeyCommand = cubaseKeyCommand;
            this.cubaseServerSettings = cubaseServerSettings;
            this.buttonName.Text = cubaseKeyCommand?.Name ?? cubaseCommand.Name;
            this.action.Text = cubaseKeyCommand?.Key ?? cubaseCommand.Action;
            this.buttonAdd.Click += ButtonAdd_Click;
            this.InitialiseAreaName();
            
            this.cbAreaName.SelectedIndexChanged += CbAreaName_SelectedIndexChanged;
            this.newAreaName.Enabled = false;
            this.newAreaName.KeyPress += NewAreaName_KeyPress;
            this.newAreaName.LostFocus += NewAreaName_LostFocus;
            this.InitialiseButtonType();
            // set default or existing colours 
            this.ButtonBackgroundColour.SetColour(this.cubaseCommand.ButtonBackgroundColour);
            this.ButtonTextColour.SetColour(this.cubaseCommand.ButtonTextColour);    
            this.ButtonToggleBackgroundColour.SetColour(this.cubaseCommand.ToggleBackGroundColour);   
            this.ButtonToggleTextColour.SetColour(this.cubaseCommand.ToggleForeColour);
            CopyColourFromArea.Click += CopyColourFromArea_Click;
        }

        private void CopyColourFromArea_Click(object? sender, EventArgs e)
        {
            ButtonBackgroundColour.SetColour(AreaBackgroundColour.JsonColour);
            ButtonTextColour.SetColour(AreaButtonTextColour.JsonColour);    
        }

        private void ButtonCubaseCommands_Click(object? sender, EventArgs e)
        {
            var macroCommandSelectorForm = new MacroCommandSelectorForm((key) => 
            {
                this.cubaseCommand.ActionGroup.Add(key.Key);
                action.Lines = cubaseCommand.ActionGroup.ToArray();

            });
            macroCommandSelectorForm.StartPosition = FormStartPosition.Manual;

            // Align left side of child to right side of parent
            macroCommandSelectorForm.Location = new Point(
                this.Location.X + this.Width,   // parent's right edge
                this.Location.Y                 // align top edges
            );
            macroCommandSelectorForm.Show(); 
        }

        private void InitialiseButtonType()
        {
            cbButtonType.Items.Clear();
            cbButtonType.Items.AddRange(Enum.GetNames(typeof(CubaseButtonType)).ToArray());
            cbButtonType.SelectedIndex = cbButtonType.Items.IndexOf(cubaseCommand.ButtonType.ToString());
            cbButtonType.SelectedIndexChanged += CbButtonType_SelectedIndexChanged;
            if (cubaseCommand.ButtonType == CubaseButtonType.Macro)
            {
                EnableMultiActions();    
            }
        }

        private void CbButtonType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var buttonType = Enum.Parse<CubaseButtonType>(cbButtonType.SelectedItem.ToString(), true);
            if (buttonType == CubaseButtonType.Macro) 
            {
                EnableMultiActions();
            }
        }

        private void EnableMultiActions()
        {
            action.Multiline = true;
            action.Lines = cubaseCommand.ActionGroup.ToArray();
            action.Height = action.Height * 2;
            action.ScrollBars = ScrollBars.Vertical;    
            this.buttonCubaseCommands.Visible = true;
        }

        private void NewAreaName_LostFocus(object? sender, EventArgs e)
        {
            if (newAreaName.Enabled)
            {
                this.AddNewArea();
            }
        }

        private void NewAreaName_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
            {
                this.AddNewArea();
            }
        }

        private void AddNewArea()
        {
            var index = (this.cbAreaName.Items.Count - 1);
            this.cbAreaName.Items.Insert(index, this.newAreaName.Text);
            this.newAreaName.Text = "";
            this.newAreaName.Enabled = false;
            this.cbAreaName.SelectedIndex = index;
        }


        private void CbAreaName_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbAreaName.SelectedIndex == (cbAreaName.Items.Count - 1))
            {
                newAreaName.Enabled = true;
            }
            else
            {
                AreaBackgroundColour.SetColour(this.cubaseCommandCollections.GetCommandCollectionByName(this.cbAreaName.SelectedItem.ToString()).BackgroundColour);
                AreaButtonTextColour.SetColour(this.cubaseCommandCollections.GetCommandCollectionByName(this.cbAreaName.SelectedItem.ToString()).TextColour);
            }
        }

        private void InitialiseAreaName()
        {
            this.cbAreaName.Items.Clear();
            this.cbAreaName.Items.AddRange(this.cubaseCommandCollections.GetNames().ToArray());
            this.cbAreaName.Items.Add("New ..");
        }

        private Color GetColour()
        {
            var colourDialog = new ColorDialog();
            if (colourDialog.ShowDialog() == DialogResult.OK) { return colourDialog.Color; }
            return Color.Empty;
        }

        private void ButtonAdd_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(buttonName.Text) && !string.IsNullOrEmpty(action.Text) && this.cbAreaName.SelectedIndex > -1)
            {
                if (this.buttonAdd.Text != "Update")
                {
                    // does area exist
                    var areaName = cbAreaName.SelectedItem as string;
                    CubaseCommandCollection commandCollection = null;
                    if (this.cubaseCommandCollections.HaveName(areaName))
                    {
                        commandCollection = cubaseCommandCollections.GetCommandCollectionByName(areaName);
                    }
                    if (commandCollection == null)
                    {
                        commandCollection = cubaseCommandCollections.WithNewCubaseCommand(areaName, "Keys")
                                                                    .WithBackgroundColour(AreaBackgroundColour.JsonColour)
                                                                    .WithTextColour(AreaButtonTextColour.JsonColour);
                    }

                    CubaseCommand cubaseCommand = CubaseCommand.CreateStandardButton(buttonName.Text, action.Text);
                    
                    switch (this.GetSelectedButtonType())
                    {
                        case CubaseButtonType.Toggle:
                            cubaseCommand = CubaseCommand.CreateToggleButton(buttonName.Text, action.Text);
                            break;
                        case CubaseButtonType.Macro:
                            var actionGroup = action.Lines.ToList();
                            cubaseCommand = CubaseCommand.CreateMacroButton(buttonName.Text, actionGroup);
                            break;
                    }
 
                    cubaseCommand.ButtonBackgroundColour = ButtonBackgroundColour.JsonColour;
                    cubaseCommand.ButtonTextColour = ButtonTextColour.JsonColour;
                    cubaseCommand.ToggleBackGroundColour = ButtonToggleBackgroundColour.JsonColour;
                    cubaseCommand.ToggleForeColour = ButtonToggleTextColour.JsonColour;

                    commandCollection.WithNewCubaseCommand(cubaseCommand);
                    cubaseCommandCollections.SaveToFile(this.cubaseServerSettings.FilePath);
                    this.Close();
                }
                else
                {
                    this.UpdateCubaseCommand();
                }
            }
        }

        private void UpdateCubaseCommand()
        {
            cubaseCommand.ButtonType = this.GetSelectedButtonType();
            cubaseCommand.ButtonTextColour = ButtonTextColour.JsonColour;
            cubaseCommand.ToggleBackGroundColour = ButtonToggleBackgroundColour.JsonColour;
            cubaseCommand.ToggleForeColour = ButtonToggleTextColour.JsonColour;
            cubaseCommand.ButtonBackgroundColour = ButtonBackgroundColour.JsonColour;
            cubaseCommand.Action = action.Text;
            cubaseCommand.Name = buttonName.Text;
            this.cubaseCommandCollections.SaveToFile(this.cubaseServerSettings.FilePath);
            this.Close();
        }

        private CubaseButtonType GetSelectedButtonType()
        {
            return Enum.Parse<CubaseButtonType>(cbButtonType.SelectedItem.ToString(), true);
        }
    }
}
