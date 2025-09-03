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

        private MomentaryDataControl momentaryDataControl;

        private MacroDataControl macroDataControl;

        private MacroToggleDataControl macroToggleDataControl; 

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
            VisibleCheckBox.Checked = true;
            this.cubaseCommand = CubaseCommand.Create();
            this.cubaseCommandCollections = cubaseCommandCollections;
            this.cbButtonType.SelectedIndex = -1;
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
            AreaButtonTest.TestButtonText = existingCommand.ParentCollectionName;
            ButtonTextColour.SetColour(existingCommand.TextColor);
            ButtonToggleBackgroundColour.SetColour(existingCommand.ToggleBackGroundColour);
            ButtonBackgroundColour.SetColour(existingCommand.ButtonBackgroundColour);
            ButtonToggleTextColour.SetColour(existingCommand.ToggleForeColour);  
            this.buttonAdd.Text = "Update";
            VisibleCheckBox.Checked = cubaseCommandCollections.GetCommandCollectionByName(this.cubaseCommand.ParentCollectionName).Visible;
        }

        private void InitialiseControls(CubaseKeyCommand cubaseKeyCommand, CubaseCommandsCollection cubaseCommandCollections, CubaseServerSettings cubaseServerSettings)
        {
            // attach colour pickers 
            AreaButtonTest.BackgroundColourPicker = AreaBackgroundColour;
            AreaButtonTest.TextColourPicker = AreaButtonTextColour;
            AreaButtonTest.Initialise();

            NormalButtonTest.BackgroundColourPicker = ButtonBackgroundColour;
            NormalButtonTest.TextColourPicker = ButtonTextColour;
            NormalButtonTest.Initialise();

            ToggleButtonTest.BackgroundColourPicker = ButtonToggleBackgroundColour;
            ToggleButtonTest.TextColourPicker = ButtonToggleTextColour;
            ToggleButtonTest.Initialise();

            this.buttonNameToggled.Text = cubaseCommand?.NameToggle ?? string.Empty;
            this.cubaseCommandCollections = cubaseCommandCollections;
            this.cubaseKeyCommand = cubaseKeyCommand;
            this.cubaseServerSettings = cubaseServerSettings;
            this.buttonName.Text = cubaseKeyCommand?.Name ?? cubaseCommand.Name;
            this.buttonName.TextChanged += ButtonName_TextChanged;
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
            NormalButtonTest.TestButtonText = this.buttonName.Text;
            ToggleButtonTest.TestButtonText = this.buttonNameToggled.Text;

        }

        private void ButtonName_TextChanged(object? sender, EventArgs e)
        {
            NormalButtonTest.TestButtonText = this.buttonName.Text;
            ToggleButtonTest.TestButtonText = this.buttonName.Text;
        }

        private void CopyColourFromArea_Click(object? sender, EventArgs e)
        {
            ButtonBackgroundColour.SetColour(AreaBackgroundColour.JsonColour);
            ButtonTextColour.SetColour(AreaButtonTextColour.JsonColour);    
        }


        private void InitialiseButtonType()
        {
            cbButtonType.Items.Clear();
            cbButtonType.Items.AddRange(Enum.GetNames(typeof(CubaseButtonType)).ToArray());
            cbButtonType.SelectedIndex = cbButtonType.Items.IndexOf(cubaseCommand.ButtonType.ToString());
            cbButtonType.SelectedIndexChanged += CbButtonType_SelectedIndexChanged;
            this.SetButtonDataPanel();
        }

        private void SetButtonDataPanel()
        {
            switch (this.GetSelectedButtonType())
            {
                case CubaseButtonType.Toggle:
                case CubaseButtonType.Momentory:
                    this.momentaryDataControl = new MomentaryDataControl();
                    this.momentaryDataControl.Text = cubaseKeyCommand?.Key ?? cubaseCommand.Action;
                    this.AddDataControl(this.momentaryDataControl);
                    break;
                case CubaseButtonType.Macro:
                    this.macroDataControl = new MacroDataControl(this.GetParentForm(this));
                    this.macroDataControl.SetCommands(cubaseCommand.ActionGroup);
                    this.AddDataControl(this.macroDataControl);
                    break;
                case CubaseButtonType.MacroToggle:
                    this.macroToggleDataControl = new MacroToggleDataControl(this.GetParentForm(this));
                    this.macroToggleDataControl.SetToogleOnCommands(cubaseCommand.ActionGroup);
                    this.macroToggleDataControl.SetToggleOffCommands(cubaseCommand.ActionGroupToggleOff);
                    this.AddDataControl(this.macroToggleDataControl);
                    break;

            }

        }

        public Control GetParentForm(Control control)
        {
            var cntrl = control;
            while (cntrl.GetType() != typeof(AddKeyToCommandsForm))
            {
                cntrl = cntrl.Parent;
            }
            return cntrl;
        }

        private void AddDataControl(Control cntrl)
        {
            this.ButtonDataPanel.Controls.Clear();
            cntrl.Dock = DockStyle.Fill;
            this.ButtonDataPanel.Controls.Add(cntrl);
        } 

        private void CbButtonType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            SetButtonDataPanel();
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
            AreaButtonTest.TestButtonText = this.newAreaName.Text;
            this.cubaseCommandCollections.WithNewCubaseCommand(this.newAreaName.Text, CubaseServiceConstants.KeyService);
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
                VisibleCheckBox.Checked = this.cubaseCommandCollections.GetCommandCollectionByName(this.cbAreaName.SelectedItem.ToString()).Visible;
            }
        }

        private void InitialiseAreaName()
        {
            this.cbAreaName.Items.Clear();
            this.cbAreaName.Items.AddRange(this.cubaseCommandCollections.GetNames().ToArray());
            this.cbAreaName.Items.Add("New ..");
        }

        private void ButtonAdd_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(buttonName.Text) && this.cbAreaName.SelectedIndex > -1)
            {
                var singleAction = string.Empty;
                if (this.buttonAdd.Text != "Update")
                {
                    switch (this.GetSelectedButtonType())
                    {
                        case CubaseButtonType.Toggle:
                        case CubaseButtonType.Momentory:
                            singleAction = this.momentaryDataControl.Text; 
                            if (string.IsNullOrEmpty(singleAction))
                            {
                                MessageBox.Show("You must select or enter a button action");
                                return;
                            }
                            break;
                    }

                    // does area exist
                    var areaName = cbAreaName.SelectedItem as string;
                    CubaseCommandCollection commandCollection = null;
                    if (this.cubaseCommandCollections.HaveName(areaName))
                    {
                        commandCollection = cubaseCommandCollections.GetCommandCollectionByName(areaName);
                    }
                    if (commandCollection == null)
                    {
                        commandCollection = cubaseCommandCollections.WithNewCubaseCommand(areaName, CubaseServiceConstants.KeyService);
                    }

                    commandCollection.WithBackgroundColour(AreaBackgroundColour.JsonColour)
                                     .WithTextColour(AreaButtonTextColour.JsonColour);

                    CubaseCommand cubaseCommand = null;
                    
                    commandCollection.Visible = VisibleCheckBox.Checked;

                    switch (this.GetSelectedButtonType())
                    {
                        case CubaseButtonType.Momentory:
                            cubaseCommand = CubaseCommand.CreateStandardButton(buttonName.Text, singleAction);
                            break;
                        case CubaseButtonType.Toggle:
                            cubaseCommand = CubaseCommand.CreateToggleButton(buttonName.Text, singleAction);
                            break;
                        case CubaseButtonType.Macro:
                            cubaseCommand = CubaseCommand.CreateMacroButton(buttonName.Text, this.macroDataControl.GetCommands());
                            cubaseCommand.ActionGroup = this.macroDataControl.GetCommands();
                            break;
                        case CubaseButtonType.MacroToggle:
                            cubaseCommand = CubaseCommand.CreateMacroToggleButton(buttonName.Text, this.macroToggleDataControl.GetToggleOnCommands(), this.macroToggleDataControl.GetToggleOffCommands());
                            cubaseCommand.ActionGroup = this.macroToggleDataControl.GetToggleOnCommands();
                            cubaseCommand.ActionGroupToggleOff = this.macroToggleDataControl.GetToggleOffCommands();
                            break;
                    }
 
                    cubaseCommand.ButtonBackgroundColour = ButtonBackgroundColour.JsonColour;
                    cubaseCommand.ButtonTextColour = ButtonTextColour.JsonColour;
                    cubaseCommand.ToggleBackGroundColour = ButtonToggleBackgroundColour.JsonColour;
                    cubaseCommand.ToggleForeColour = ButtonToggleTextColour.JsonColour;

                    cubaseCommand.NameToggle = string.IsNullOrEmpty(buttonNameToggled.Text) ? buttonName.Text : buttonNameToggled.Text; 

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
            cubaseCommandCollections.GetCommandCollectionByName(this.cubaseCommand.ParentCollectionName).Visible = VisibleCheckBox.Checked;
            cubaseCommand.ButtonType = this.GetSelectedButtonType();
            cubaseCommand.ButtonTextColour = ButtonTextColour.JsonColour;
            cubaseCommand.ToggleBackGroundColour = ButtonToggleBackgroundColour.JsonColour;
            cubaseCommand.ToggleForeColour = ButtonToggleTextColour.JsonColour;
            cubaseCommand.ButtonBackgroundColour = ButtonBackgroundColour.JsonColour;
            switch (this.GetSelectedButtonType())
            {
                case CubaseButtonType.Momentory:
                    cubaseCommand.Action = this.momentaryDataControl.Text;
                    break;
                case CubaseButtonType.Toggle:
                    cubaseCommand.Action = this.momentaryDataControl.Text;
                    break;
                case CubaseButtonType.Macro:
                    cubaseCommand.ActionGroup = this.macroDataControl.GetCommands();
                    break;
                case CubaseButtonType.MacroToggle:
                    cubaseCommand.ActionGroup = this.macroToggleDataControl.GetToggleOnCommands();
                    cubaseCommand.ActionGroupToggleOff = this.macroToggleDataControl.GetToggleOffCommands();
                    break;
            }
            cubaseCommand.Name = buttonName.Text;
            cubaseCommand.NameToggle = buttonNameToggled.Text ?? buttonName.Text;
            this.cubaseCommandCollections.SaveToFile(this.cubaseServerSettings.FilePath);
            this.Close();
        }

        private CubaseButtonType GetSelectedButtonType()
        {
            return Enum.Parse<CubaseButtonType>(cbButtonType.SelectedItem.ToString(), true);
        }
    }
}
