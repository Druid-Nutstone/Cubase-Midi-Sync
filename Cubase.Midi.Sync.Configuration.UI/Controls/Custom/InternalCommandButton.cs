using Cubase.Midi.Sync.Common.InternalCommands;
using Cubase.Midi.Sync.Configuration.UI.Controls.InternalCommands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Custom
{
    public class InternalCommandButton : Button
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<InternalCommand> OnCommand { get; set; }   
        
        public InternalCommandButton() : base()
        {
            base.Text = "Internal Command"; 
        }

        protected override void OnClick(EventArgs e)
        {
            var internalCommandForm = new InternalCommandForm(this.OnCommand);
            internalCommandForm.ShowDialog();
        }
    }
}
