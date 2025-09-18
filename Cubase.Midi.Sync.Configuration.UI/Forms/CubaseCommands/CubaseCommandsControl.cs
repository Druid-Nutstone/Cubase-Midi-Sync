using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Forms.CubaseCommands
{
    public partial class CubaseCommandsControl : UserControl
    {
        public CubaseKnownCollection allCommands;
        
        public CubaseCommandsControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.allCommands = new CubaseKnownCollection();
            this.cubaseCommandsListView.Populate(this.allCommands);
            this.searchFilter.TextChanged += SearchFilter_TextChanged;
            this.ShowAssignedCheckBox.CheckedChanged += ShowAssignedCheckBox_CheckedChanged;
        }

        private void ShowAssignedCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (this.ShowAssignedCheckBox.Checked)
            {
                var assigned = this.allCommands.Where(x => !string.IsNullOrEmpty(x.CommandBinding));
                this.cubaseCommandsListView.Populate(assigned.ToList());
            }
            else
            {
                this.cubaseCommandsListView.Populate(this.allCommands);
            }
        }

        private void SearchFilter_TextChanged(object? sender, EventArgs e)
        {
            if (this.searchFilter.Text.Length > 3)
            {
                var searchResult = this.allCommands.Where(x => x.CommandDescription.Contains(searchFilter.Text, StringComparison.OrdinalIgnoreCase));
                this.cubaseCommandsListView.Populate(searchResult.ToList());
            }
            else
            {
                if (this.searchFilter.Text.Length == 0)
                {
                    this.cubaseCommandsListView.Populate(this.allCommands);
                }
            }
        }
    }
}
