using Cubase.Midi.Sync.Common.Midi;
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

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Midi
{
    public partial class MidiCommandSelectorForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CloseAfterSelect { get; set; } = false;


        public MidiCommandSelectorForm()
        {
            InitializeComponent();
        }

        public MidiCommandSelectorForm(Action<CubaseMidiCommand> keyHandler)
        {
            InitializeComponent();
            var midiCommands = new CubaseMidiCommandCollection(CubaseConfigurationConstants.KeyCommandsFileLocation);
            this.midiCommandListView.Populate(midiCommands, (key)=> 
            {
                keyHandler(key);
                if (this.CloseAfterSelect)
                {
                    this.Close();
                }       
            });
        }
    }
}
