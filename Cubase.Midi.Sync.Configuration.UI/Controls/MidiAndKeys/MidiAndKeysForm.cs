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

namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiAndKeys
{
    public partial class MidiAndKeysForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CloseAfterSelect { get; set; } = false;

        public MidiAndKeysForm()
        {
            InitializeComponent();
        }

        public MidiAndKeysForm(Action<MidiAndKey> keyHandler)
        {
            InitializeComponent();
            this.midiAndKeysControl.Initialise((cmd) => 
            {
                keyHandler(cmd);
                if (this.CloseAfterSelect)
                {
                    this.Close();
                }
            });
        }

    }
}
