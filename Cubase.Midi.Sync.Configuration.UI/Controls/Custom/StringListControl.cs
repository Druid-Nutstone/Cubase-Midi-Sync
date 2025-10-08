using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Custom
{
    public class StringListControl : ListBox
    {
        public StringListControl() : base() 
        {
            this.DisplayMember = nameof(ActionEvent.Action);
        }

        public void Populate(List<ActionEvent> list)
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

        public void PopulateSingle(ActionEvent action)
        {
            this.Items.Add(action);
        }

        public List<ActionEvent> GetCommands()
        {
            var result = new List<ActionEvent>();
            foreach (var item in this.Items)
            {
                result.Add((ActionEvent)item);
            }
            return result;
        }

        public void Remove()
        {
            if (this.SelectedIndex < 0) return; 
            this.Items.RemoveAt(this.SelectedIndex);
        }

    }
}
