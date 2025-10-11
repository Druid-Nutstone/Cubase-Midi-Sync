using Cubase.Midi.Sync.Common;
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

        public AreaMainControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.MoveUp.Click += MoveUp_Click; 
            this.MoveDown.Click += MoveDown_Click;  
            this.AddCategory.Click += AddCategory_Click;  
            this.DeleteCategory.Click += DeleteCategory_Click;
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
        }
    }
}
