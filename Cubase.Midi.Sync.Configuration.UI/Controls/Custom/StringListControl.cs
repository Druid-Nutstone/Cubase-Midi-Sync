using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Custom
{
    public class StringListControl : ListBox
    {
        public StringListControl() : base() { }

        public void Popsulate(List<string> list)
        {
            if (list.Count > 0)
            {
                this.Items.AddRange([.. list]);
            }
        }

        public List<string> GetList()
        {
            return this.Items.Cast<string>().ToList();
        }

        public void PopulateSingle(string text)
        {
            this.Items.Add(text);
        }

        public void Remove()
        {
            if (this.SelectedIndex < 0) return; 
            this.Items.RemoveAt(this.SelectedIndex);
        }

    }
}
