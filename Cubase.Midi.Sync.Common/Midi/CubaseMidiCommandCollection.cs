using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cubase.Midi.Sync.Common.Midi
{
    public class CubaseMidiCommandCollection : List<CubaseMidiCommand>
    {
        public CubaseMidiCommandCollection() 
        {
            this.Add(CubaseMidiCommand.Create("Rewind To Start", 0, 1, "Transport", "Return to Zero", 127));
            this.Add(CubaseMidiCommand.Create("Start", 0, 2, "Transport", "Start", 127));
            this.Add(CubaseMidiCommand.Create("Stop", 0, 3, "Transport", "Stop", 127));
            this.Add(CubaseMidiCommand.Create("Punch In", 0, 4, "Transport", "Auto Punch In", 127));
            this.Add(CubaseMidiCommand.Create("Punch Out", 0, 5, "Transport", "Auto Punch Out", 127));
            this.Add(CubaseMidiCommand.Create("Set Marker 1", 0, 6, "Transport", "Set Marker 1", 127));
            this.Add(CubaseMidiCommand.Create("Set Marker 2", 0, 7, "Transport", "Set Marker 2", 127));
            this.Add(CubaseMidiCommand.Create("Go To Marker 1", 0, 8, "Transport", "To Marker 1", 127));
            this.Add(CubaseMidiCommand.Create("Go To Marker 2", 0, 9, "Transport", "To Marker 2", 127));
            this.Add(CubaseMidiCommand.Create("Show Tracks With Data", 0, 10, "Channel & Track Visibility", "ShowUsed", 127));
            this.Add(CubaseMidiCommand.Create("Show All Tracks", 0, 11, "Channel & Track Visibility", "ShowAll", 127));
            this.Add(CubaseMidiCommand.Create("Show Selected Tracks", 0, 12, "Channel & Track Visibility", "ShowSelected", 127));
            this.Add(CubaseMidiCommand.Create("Zoom Selected Tracks", 0, 13, "Zoom", "Zoom Tracks Exclusive", 127));
            this.Add(CubaseMidiCommand.Create("Undo Zoom", 0, 14, "Zoom", "Undo Zoom", 127));
            this.Add(CubaseMidiCommand.Create("Record Enable", 0, 15, "Edit", "Record Enable", 127));
            this.Add(CubaseMidiCommand.Create("Collapse Tracks", 0, 16, "Project", "Folding: Fold Tracks", 127));
            this.Add(CubaseMidiCommand.Create("Expand Tracks", 0, 17, "Project", "Folding: Unfold Tracks", 127));
            this.Add(CubaseMidiCommand.Create("Mixer", 0, 18, "Devices", "Mixer", 127));
            this.Add(CubaseMidiCommand.Create("Hide Audio", 0, 19, "Mixer", "Hide: Audio", 127));
            this.Add(CubaseMidiCommand.Create("Hide Groups", 0, 20, "Mixer", "Hide: Groups", 127));
            this.Add(CubaseMidiCommand.Create("Hide Inputs", 0, 21, "Mixer", "Hide: Inputs", 127));
            this.Add(CubaseMidiCommand.Create("Hide Instruments", 0, 22, "Mixer", "Hide: Instruments", 127));
            this.Add(CubaseMidiCommand.Create("Hide Midi", 0, 23, "Mixer", "Hide: MIDI", 127));
            this.Add(CubaseMidiCommand.Create("Hide Outputs", 0, 24, "Mixer", "Hide: Outputs", 127));
            this.Add(CubaseMidiCommand.Create("Mixer Show All", 0, 25, "Mixer", "Hide: Reveal All", 127));

        }

        public CubaseMidiCommand GetCommandByCommand(string command)
        {
            return this.FirstOrDefault(x => x.Command.Equals(command, StringComparison.OrdinalIgnoreCase));
        }

    }

    public class CubaseMidiCommand
    {
        public string Name { get; set; }
        public int Channel { get; set; }
        public int Note { get; set; }
        public string Category { get; set; }
        public string Command { get; set; }

        public int Velocity { get; set; }
        
        public static CubaseMidiCommand Create(string name, int channel, int note, string category, string command, int velocity)
        {
            return new CubaseMidiCommand()
            {
                Name = name,
                Channel = channel,
                Note = note,
                Category = category,
                Command = command,
                Velocity = velocity
            };
        }
    }
}
