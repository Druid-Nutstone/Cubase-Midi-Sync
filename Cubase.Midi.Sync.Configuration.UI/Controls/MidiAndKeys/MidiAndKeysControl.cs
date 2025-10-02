using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiAndKeys
{
    public partial class MidiAndKeysControl : UserControl
    {
        private MidiAndKeysCollection collection;

        private Action<MidiAndKey> keyHandler;

        public MidiAndKeysControl()
        {
            InitializeComponent();
        }

        public void Initialise(Action<MidiAndKey> keyHandler)
        {
            this.keyHandler = keyHandler;
            this.InitialiseForm();
            this.collection = new MidiAndKeysCollection();
            this.midiAndKeysListView.Populate(collection, keyHandler);
            this.InitialiseForm();
        }

        private void InitialiseForm()
        {
            this.SelectTypeCombo.Items.Clear();
            this.SelectTypeCombo.Items.AddRange(Enum.GetNames<CubaseAreaTypes>());
            this.SelectTypeCombo.SelectedIndexChanged += SelectTypeCombo_SelectedIndexChanged;
            this.ClearTypeButton.Click += ClearTypeButton_Click;
            this.ClearSearchButton.Click += ClearTypeButton_Click;
            this.SearchText.TextChanged += SearchText_TextChanged;
        }

        private void SearchText_TextChanged(object? sender, EventArgs e)
        {
            if (this.SearchText.Text.Length > 2)
            {
                var searchResult = this.collection.Where(x => x.Name.Contains(this.SearchText.Text, StringComparison.OrdinalIgnoreCase) ||
                                           x.Category.Contains(this.SearchText.Text, StringComparison.OrdinalIgnoreCase) ||
                                           x.Action.Contains(this.SearchText.Text, StringComparison.OrdinalIgnoreCase))
                                                  .ToList();
                if (searchResult.Count > 0)
                {
                    this.midiAndKeysListView.Populate(searchResult, this.keyHandler);
                }
            }
        }

        private void ClearTypeButton_Click(object? sender, EventArgs e)
        {
            this.SearchText.Text = string.Empty;
            this.SelectTypeCombo.SelectedIndex = -1;
            this.midiAndKeysListView.Populate(this.collection, keyHandler);
        }

        private void SelectTypeCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (this.SelectTypeCombo.SelectedIndex > -1)
            {
                var asType = Enum.Parse<CubaseAreaTypes>(this.SelectTypeCombo.SelectedItem?.ToString());
                var selectionType = this.collection.Where(x => x.KeyType == asType).ToList();
                this.midiAndKeysListView.Populate(selectionType, this.keyHandler);
            }

        }
    }
}
