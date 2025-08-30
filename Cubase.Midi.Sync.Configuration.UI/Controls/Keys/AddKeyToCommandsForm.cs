using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Extensions;
using Cubase.Midi.Sync.Configuration.UI.Controls.Macros;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            this.InitialiseControls(cubaseKeyCommand, cubaseCommandCollections, cubaseServerSettings);
            this.cbAreaName.SelectedIndex = cubaseCommandCollections.FindIndex(x => x.Name == category);
            this.cbAreaName.Enabled = false;
            textColour.Text = existingCommand.TextColor.FromSerializableColour().ToArgb().ToString();
            toggleBackgroundColour.Text = existingCommand.ToggleBackGroundColour.FromSerializableColour().ToArgb().ToString();      
            backgroundColour.Text = existingCommand.ButtonBackgroundColour.FromSerializableColour().ToArgb().ToString();    
            toggleTextColour.Text = existingCommand.ToggleForeColour.FromSerializableColour().ToArgb().ToString();  
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
            this.backgroundColourButton.Click += BackgroundColourButton_Click;
            this.textColourButton.Click += TextColourButton_Click;
            this.InitialiseAreaName();
            
            this.cbAreaName.SelectedIndexChanged += CbAreaName_SelectedIndexChanged;
            this.newAreaName.Enabled = false;
            this.newAreaName.KeyPress += NewAreaName_KeyPress;
            this.newAreaName.LostFocus += NewAreaName_LostFocus;
            this.toggleBackgroundColourButton.Click += ToggleBackgroundColourButton_Click;
            this.toggleTextColourButton.Click += ToggleTextColourButton_Click;
            this.InitialiseButtonType();
            // set default or existing colours 
            this.backgroundColour.BackColor = this.cubaseCommand.ButtonBackgroundColour.FromSerializableColour();
            this.textColour.BackColor = this.cubaseCommand.ButtonTextColour.FromSerializableColour();    
            this.toggleBackgroundColour.BackColor = this.cubaseCommand.ToggleBackGroundColour.FromSerializableColour();   
            this.toggleTextColour.BackColor = this.cubaseCommand.ToggleForeColour.FromSerializableColour(); 
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

        private void ToggleTextColourButton_Click(object? sender, EventArgs e)
        {
            var clr = GetColour();
            if (clr != Color.Empty)
            {
                this.toggleTextColour.BackColor = clr;
                this.toggleTextColour.Text = clr.ToArgb().ToString();
            }
        }

        private void ToggleBackgroundColourButton_Click(object? sender, EventArgs e)
        {
            var clr = GetColour();
            if (clr != Color.Empty)
            {
                this.toggleBackgroundColour.BackColor = clr;
                this.toggleBackgroundColour.Text = clr.ToArgb().ToString();
            }
        }



        private void CbAreaName_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbAreaName.SelectedIndex == (cbAreaName.Items.Count - 1))
            {
                newAreaName.Enabled = true;
            }
        }

        private void TextColourButton_Click(object? sender, EventArgs e)
        {
            var clr = GetColour();
            if (clr != Color.Empty)
            {
                this.textColour.BackColor = clr;
                this.textColour.Text = clr.ToArgb().ToString();
            }
        }

        private void BackgroundColourButton_Click(object? sender, EventArgs e)
        {
            var clr = GetColour();
            if (clr != Color.Empty)
            {
                this.backgroundColour.BackColor = clr;
                this.backgroundColour.Text = clr.ToArgb().ToString();
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
                        commandCollection = cubaseCommandCollections.WithNewCubaseCommand(areaName, "Keys");
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
 
                    if (!string.IsNullOrEmpty(backgroundColour.Text))
                    {
                        cubaseCommand.ButtonBackgroundColour = Color.FromArgb(int.Parse(backgroundColour.Text)).ToSerializableColour();
                    }

                    if (!string.IsNullOrEmpty(textColour.Text))
                    {
                        cubaseCommand.ButtonTextColour = Color.FromArgb(int.Parse(textColour.Text)).ToSerializableColour();
                    }

                    if (!string.IsNullOrEmpty(toggleBackgroundColour.Text))
                    {
                        cubaseCommand.ToggleBackGroundColour = Color.FromArgb(int.Parse(toggleBackgroundColour.Text)).ToSerializableColour();
                    }

                    if (!string.IsNullOrEmpty(toggleTextColour.Text))
                    {
                        cubaseCommand.ToggleForeColour = Color.FromArgb(int.Parse(toggleTextColour.Text)).ToSerializableColour();
                    }

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
            cubaseCommand.ButtonTextColour = Color.FromArgb(int.Parse(textColour.Text)).ToSerializableColour();
            cubaseCommand.ToggleBackGroundColour = Color.FromArgb(int.Parse(toggleBackgroundColour.Text)).ToSerializableColour();
            cubaseCommand.ToggleForeColour = Color.FromArgb(int.Parse(toggleTextColour.Text)).ToSerializableColour();
            cubaseCommand.ButtonBackgroundColour = Color.FromArgb(int.Parse(backgroundColour.Text)).ToSerializableColour();
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
