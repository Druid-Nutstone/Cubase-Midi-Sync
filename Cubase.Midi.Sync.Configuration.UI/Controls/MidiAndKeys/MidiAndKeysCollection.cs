using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Keys;
using Cubase.Midi.Sync.Common.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.MidiAndKeys
{
    public class MidiAndKeysCollection : List<MidiAndKey>
    {
        public MidiAndKeysCollection() 
        {
            var cubaseKeyCommands = new CubaseKeyCommandParser().Parse(CubaseConfigurationConstants.KeyCommandsFileLocation).GetAllocated();
            var midiCommands = new CubaseMidiCommandCollection(CubaseConfigurationConstants.KeyCommandsFileLocation);
            midiCommands.ForEach(m =>
            {
                this.Add(MidiAndKey.AddMidi(m.Name, m.Command, m.Category));
            });
            cubaseKeyCommands.ForEach(x => 
            {
                this.Add(MidiAndKey.AddKey(x.Name, x.Key, x.Category));
            });

        }

    }



}
