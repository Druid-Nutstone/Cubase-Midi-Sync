using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Keys;
using Cubase.Midi.Sync.Common.Scripts;
using Cubase.Midi.Sync.Common.SysEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Midi
{
    public class MidiAndKeysCollection : List<MidiAndKey>
    {
        
        
        public MidiAndKeysCollection() 
        {
            var cubaseKeyCommands = new CubaseKeyCommandParser().Parse(CubaseServerSettings.KeyCommandsFileLocation).GetAllocated();
            var midiCommands = new CubaseMidiCommandCollection(CubaseServerSettings.KeyCommandsFileLocation);
            var scriptCommands = new CubaseScriptCollection();
            midiCommands.ForEach(m =>
            {
                this.Add(MidiAndKey.AddMidi(m.Name, m.Command, m.Category));
            });
            cubaseKeyCommands.ForEach(x => 
            {
                this.Add(MidiAndKey.AddKey(x.Name, x.Key, x.Category));
            });
            scriptCommands.ForEach(s => 
            {
                this.Add(MidiAndKey.AddScript(s.Name, s.FileName));
            });
            var sysExCommands = Enum.GetNames<SysExCommand>();
            foreach (var sysExCommand in sysExCommands)
            {
                this.Add(MidiAndKey.AddSysEx(Enum.Parse<SysExCommand>(sysExCommand)));
            }

        }

        public MidiAndKey GetMidiCommand(KnownCubaseMidiCommands midiCommand)
        {
            var actualName = midiCommand.ToString().Replace("_", " ").Trim();
            return this.FirstOrDefault(x => x.Name.Equals(actualName, StringComparison.OrdinalIgnoreCase));
        }

        public MidiAndKey GetCommandByName(string name) 
        { 
            return this.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

    }



}
