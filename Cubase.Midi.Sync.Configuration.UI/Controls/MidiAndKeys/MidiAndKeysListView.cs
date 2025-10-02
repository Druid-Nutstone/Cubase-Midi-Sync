
using Cubase.Midi.Sync.Configuration.UI.Controls.Midi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiAndKeys
{
    public class MidiAndKeysListView : ListView
    {

        private Action<MidiAndKey> keyHandler;

        private ListViewColumnSorter lvwColumnSorter;

        public MidiAndKeysListView() : base()
        {
            this.View = View.Details;
            this.MultiSelect = false;
            this.DoubleBuffered = true;
            this.FullRowSelect = true;
            this.AddHeader("Type");
            this.AddHeader("Name");
            this.AddHeader("Command");
            this.AddHeader("Category");
            this.lvwColumnSorter = new ListViewColumnSorter();
            this.ListViewItemSorter = lvwColumnSorter;
        }

        public void AddHeader(string text)
        {
            this.Columns.Add(text);
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                lvwColumnSorter.Order = lvwColumnSorter.Order == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                // Set the column number to sort by
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            this.Sort();
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (this.SelectedItems.Count > 0)
            {
                var selectedItem = this.SelectedItems[0] as MidiAndKeysListViewItem;
                if (selectedItem != null)
                {
                    this.keyHandler(selectedItem.MidiAndKeyCommand);
                }
            }
        }

        public void Populate(List<MidiAndKey> midiAndKeys, Action<MidiAndKey> keyHandler)
        {
            this.keyHandler = keyHandler;
            this.Items.Clear();
            midiAndKeys.ForEach(k =>
            {
                this.Items.Add(new MidiAndKeysListViewItem(k));
            });
            this.AutoFit();
        }

        private void AutoFit()
        {
            foreach (ColumnHeader column in this.Columns)
            {
                // Measure header width
                this.AutoResizeColumn(column.Index, ColumnHeaderAutoResizeStyle.HeaderSize);
                int headerWidth = column.Width;

                // Measure content width
                this.AutoResizeColumn(column.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
                int contentWidth = column.Width;

                // Pick whichever is larger
                column.Width = Math.Max(headerWidth, contentWidth);
            }
        }
    }

    public class MidiAndKeysListViewItem : ListViewItem
    {
        public MidiAndKey MidiAndKeyCommand { get; set; }

        public MidiAndKeysListViewItem(MidiAndKey midiAndKey)
        {
            this.MidiAndKeyCommand = midiAndKey;
            this.Text = midiAndKey.KeyType.ToString();
            this.SubItems.Add(midiAndKey.Name);
            this.SubItems.Add(midiAndKey.Action);
            this.SubItems.Add(midiAndKey.Category);
        }
    }

    public class ListViewColumnSorter : IComparer
    {
        public int SortColumn { get; set; }
        public SortOrder Order { get; set; }

        private CaseInsensitiveComparer ObjectCompare;

        public ListViewColumnSorter()
        {
            SortColumn = 0;
            Order = SortOrder.None;
            ObjectCompare = new CaseInsensitiveComparer();
        }

        public int Compare(object x, object y)
        {
            ListViewItem itemX = (ListViewItem)x;
            ListViewItem itemY = (ListViewItem)y;

            string valueX = itemX.SubItems[SortColumn].Text;
            string valueY = itemY.SubItems[SortColumn].Text;

            int result;

            // Try to compare as numbers first
            if (double.TryParse(valueX, out double numX) && double.TryParse(valueY, out double numY))
            {
                result = numX.CompareTo(numY);
            }
            else
            {
                result = ObjectCompare.Compare(valueX, valueY);
            }

            return Order == SortOrder.Ascending ? result : -result;
        }
    }
}
