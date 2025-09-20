using Cubase.Midi.Sync.Common.Keys;
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

namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiMacros.Forms
{
    public partial class AddMidiMacroForm : Form
    {
        private CubaseMacroCommandCollection macroCommands;
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CloseAfterSelect { get; set; } = false;

        public AddMidiMacroForm()
        {
            InitializeComponent();
        }

        public AddMidiMacroForm(Action<CubaseMacroCommand> keyHandler)
        {
            InitializeComponent();
            //this.macroCommands = CubaseKeyCommandParser.Create().ParseMacros();
            //this.midiMacroSelectListView.Populate(this.macroCommands, (command) => 
            //{ 
            //    keyHandler(command);    
            //    if (this.CloseAfterSelect)
            //    {
            //        this.Close();
            //    }
            //});
        }
    }
}
