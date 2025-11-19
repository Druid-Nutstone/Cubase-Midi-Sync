using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Keys;
using Cubase.Midi.Sync.Configuration.UI.Controls.Keys;
using Cubase.Midi.Sync.Configuration.UI.Controls.MidiAndKeys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Areas
{
    public partial class AreaMainControl : UserControl
    {
        private CubaseServerSettings cubaseServerSettings;
        
        private CubaseCommandsCollection commands;

        private CubaseCommandCollection command;

        private string buttonCategory;

        private PrePostCommand preCommandName;

        private PrePostCommand postCommandName;

        public AreaMainControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.MoveUp.Click += MoveUp_Click; 
            this.MoveDown.Click += MoveDown_Click;  
            this.AddCategory.Click += AddCategory_Click;  
            this.DeleteCategory.Click += DeleteCategory_Click;
            this.PreCommandUp.Click += PreCommandUp_Click;
            this.PreCommandAdd.Click += PreCommandAdd_Click;
            this.PostCommandAdd.Click += PostCommandAdd_Click;
        }

        private void PostCommandAdd_Click(object? sender, EventArgs e)
        {
            this.GetMidiOrKeyCommand((key) =>
            {
                this.command.PostCommands.Add(PrePostCommand.CreateFromMidiAndKey(key));
                this.SaveCollection();
                this.PopulatePrePostCommands(this.command);
            });
        }

        private void PreCommandAdd_Click(object? sender, EventArgs e)
        {
            this.GetMidiOrKeyCommand((key) => 
            {
                this.command.PreCommands.Add(PrePostCommand.CreateFromMidiAndKey(key));
                this.SaveCollection();
                this.PopulatePrePostCommands(this.command);
            });
        }

        private void GetMidiOrKeyCommand(Action<MidiAndKey> action)
        {
            var parentForm = this.GetParentForm(this);
            MidiAndKeysForm form;
            form = new MidiAndKeysForm(action);
            form.StartPosition = FormStartPosition.Manual;
            form.CloseAfterSelect = true;
            parentForm.Move += (sender, e) =>
            {
                form.Location = new Point(
                    parentForm.Bounds.Right,   // right edge in screen coordinates
                    parentForm.Bounds.Top      // top edge in screen coordinates
               );
            };

            // Align left side of child to right side of parent
            form.Location = new Point(
                 parentForm.Bounds.Right,   // right edge in screen coordinates
                 parentForm.Bounds.Top      // top edge in screen coordinates
            );
            form.Show();
        }

        private void PreCommandUp_Click(object? sender, EventArgs e)
        {
            if (this.command != null && this.command.PreCommands.Count > 1)
            {
                int index = this.command.PreCommands.IndexOf(this.preCommandName);
                if (index > 0)
                {
                    this.command.PreCommands.RemoveAt(index);
                    this.command.PreCommands.Insert(index - 1, this.preCommandName);
                    this.SaveCollection();
                    this.PopulatePrePostCommands(this.command);
                }
            }
        }

        private void DeleteCategory_Click(object? sender, EventArgs e)
        {
            if (this.command != null && this.buttonCategory != null)
            {
                this.command.ButtonCategories.Remove(this.buttonCategory);
                
                this.command.Commands
                    .Where(c => c.ButtonCategory == this.buttonCategory)
                    .ToList()
                    .ForEach(c => c.ButtonCategory = string.Empty);

                this.SaveCollection();
                this.PopulateButtonCategories(this.command);
            }
        }

        private void AddCategory_Click(object? sender, EventArgs e)
        {
            if (this.command != null)
            {
                this.command.ButtonCategories.Add(this.NewCategoryText.Text);
                this.SaveCollection();
                this.PopulateButtonCategories(this.command);
                this.NewCategoryText.Text = string.Empty;   
            }
        }

        private void SaveCollection()
        {
            if (this.cubaseServerSettings != null && this.commands != null)
            {
                this.commands.SaveToFile(this.cubaseServerSettings.FilePath);   
            }
        }

        private void MoveDown_Click(object? sender, EventArgs e)
        {
            if (this.command != null && this.command.ButtonCategories.Count > 1)
            {
                int index = this.command.ButtonCategories.IndexOf(this.buttonCategory);
                if (index < this.command.ButtonCategories.Count - 1)
                {
                    this.command.ButtonCategories.RemoveAt(index);
                    this.command.ButtonCategories.Insert(index + 1, this.buttonCategory);
                    this.SaveCollection();
                    this.PopulateButtonCategories(this.command);
                }
            }
        }

        private void MoveUp_Click(object? sender, EventArgs e)
        {
            if (this.command != null && this.command.ButtonCategories.Count > 1)
            {
                int index = this.command.ButtonCategories.IndexOf(this.buttonCategory);
                if (index > 0)
                {
                    this.command.ButtonCategories.RemoveAt(index);
                    this.command.ButtonCategories.Insert(index - 1, this.buttonCategory);
                    this.SaveCollection();
                    this.PopulateButtonCategories(this.command);
                }
            }
        }

        public void Populate()
        {
            this.cubaseServerSettings = new CubaseServerSettings();
            this.commands = cubaseServerSettings.GetCubaseCommands();
            this.areaListView.Populate(this.commands, this.cubaseServerSettings, this.SelectionHandler);
        
        }

        private void PopulatePrePostCommands(CubaseCommandCollection command)
        {
            this.PreCommandListBox.Items.Clear();
            this.PreCommandListBox.DisplayMember = nameof(PrePostCommand.Name);
            this.PreCommandListBox.Items.AddRange(command.PreCommands.ToArray());
            this.PreCommandListBox.SelectedIndexChanged += PreCommandListBox_SelectedIndexChanged;

            this.PostCommandListBox.Items.Clear();
            this.PostCommandListBox.DisplayMember = nameof(PrePostCommand.Name);
            this.PostCommandListBox.Items.AddRange(command.PostCommands.ToArray());
            this.PostCommandListBox.SelectedIndexChanged += PostCommandListBox_SelectedIndexChanged;
        }

        private void PostCommandListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            this.postCommandName = this.PostCommandListBox.SelectedItem as PrePostCommand;
        }

        private void PreCommandListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            this.preCommandName = this.PreCommandListBox.SelectedItem as PrePostCommand;
        }

        private void PopulateButtonCategories(CubaseCommandCollection command)
        {
            this.ButtonCategoryListBox.Items.Clear();   
            this.ButtonCategoryListBox.Items.AddRange(command.ButtonCategories.ToArray());
            this.ButtonCategoryListBox.SelectedIndexChanged += ButtonCategoryListBox_SelectedIndexChanged;
        }

        private void ButtonCategoryListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            this.buttonCategory = this.ButtonCategoryListBox.SelectedItem as string;
        }

        private void SelectionHandler(CubaseCommandCollection command)
        {
            this.command = command; 
            this.PopulateButtonCategories(command); 
            this.PopulatePrePostCommands(command);  
        }

        private Control GetParentForm(Control control)
        {
            var cntrl = control;
            while (cntrl.GetType() != typeof(Form1))
            {
                if (cntrl.Parent != null)
                {
                    cntrl = cntrl.Parent;
                }
                else
                {
                    return cntrl;
                }
            }
            return cntrl;
        }
    }
}
