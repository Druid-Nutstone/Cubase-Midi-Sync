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

namespace Cubase.Midi.Sync.Configuration.UI.Controls
{
    public partial class KeysMainControl : UserControl
    {
        private CubaseKeyCommandCollection commands;       
        public KeysMainControl()
        {
            InitializeComponent();
            searchButton.Click += SearchButton_Click;
            this.Dock = DockStyle.Fill;
            this.commands = CubaseKeyCommandParser.Create().Parse();

            this.ListViewPanel.Controls.Clear();
            var keysTreeView = new KeysTreeView();
            this.ListViewPanel.Controls.Add(keysTreeView);
            keysTreeView.Populate(this.commands, this.DataPanel);
            searchInput.KeyPress += SearchInput_KeyPress;
        }

        private void SearchInput_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
            {
                SearchButton_Click(sender, e);  
            } 
        }

        private void SearchButton_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(searchInput.Text))
            {
                this.DataPanel.Controls.Clear();
                var results = this.commands.GetByName(searchInput.Text);    
                if (results.Count > 0)
                {
                    var keysListView = new KeysListView();
                    this.DataPanel.Controls.Add(keysListView);
                    keysListView.Populate(results);
                }
                else
                {
                    MessageBox.Show("No results found", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }

        }
    }
}
