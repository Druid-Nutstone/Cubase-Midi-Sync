using Cubase.Midi.Sync.Common.Keys;
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
        public CubaseMidiCommandCollection(string keyCommandFileLocation)
        {
            var requiredKeys = RequiredKeyMappingCollection.Create(null, keyCommandFileLocation);
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
            this.Add(CubaseMidiCommand.Create("Next Track", 0, 26, "Project", "Select Track: Next", 127));
            this.Add(CubaseMidiCommand.Create("Previous Track", 0, 27, "Project", "Select Track: Prev", 127));
            this.Add(CubaseMidiCommand.Create("Zoom Out", 1, 1, "Zoom", "Zoom Out", 127));
            this.Add(CubaseMidiCommand.Create("Zoom In", 1, 2, "Zoom", "Zoom In", 127));
            this.Add(CubaseMidiCommand.Create("Enlarge Selected Track", 1, 3, "Edit", "Enlarge Selected Track", 127));
            this.Add(CubaseMidiCommand.Create("Key Hide All", -1, 0, "Mixer", requiredKeys.GetKey(RequiredKeyId.Mixer_Hide_All), 127));
            this.Add(CubaseMidiCommand.Create("Key Show All", -1, 0, "Mixer", requiredKeys.GetKey(RequiredKeyId.Mixer_Show_All), 127));
            this.Add(CubaseMidiCommand.Create("Zoom To Selection", 1, 4, "Zoom", "Zoom to Selection", 127));
            this.Add(CubaseMidiCommand.Create("Go to Left Locator Position", 1, 5, "Transport", "To Left Locator", 127));
            this.Add(CubaseMidiCommand.Create("Go to Right Locator Position", 1, 6, "Transport", "To Right Locator", 127));
            this.Add(CubaseMidiCommand.Create("Re-Record on/off", 1, 7, "Transport", "Re-Record on/off", 127));
            this.Add(CubaseMidiCommand.Create("Edit Undo", 1, 8, "Edit", "Undo", 127));
            this.Add(CubaseMidiCommand.Create("New Track Version", 1, 9, "TrackVersions", "New Version", 127));
            this.Add(CubaseMidiCommand.Create("Select Tracks With Selected Event", 1, 10, "Channel & Track Visibility", "ShowMarked", 127));
            this.Add(CubaseMidiCommand.Create("Hide Muted Tracks", 1, 11, "Channel & Track Visibility", "HideMuted", 127));
            this.Add(CubaseMidiCommand.Create("MetroNone On/Off", 1, 12, "Transport", "Metronome On", 127));
            this.Add(CubaseMidiCommand.Create("Set Left Locator", 1, 13, "Transport", "Set Left Locator", 127));
            this.Add(CubaseMidiCommand.Create("Set Right Locator", 1, 14, "Transport", "Set Right Locator", 127));
            this.Add(CubaseMidiCommand.Create("Loop Selection", 1, 15, "Transport", "Loop Selection", 127));
        }


        public CubaseMidiCommand GetCommandByCommand(string command)
        {
            return this.FirstOrDefault(x => x.Command.Equals(command, StringComparison.OrdinalIgnoreCase));
        }

        public string GetCommandByName(KnownCubaseMidiCommands name)
        {
            var found = this.FirstOrDefault(x => x.Name.Equals(name.ToString().Replace("_", " ")));
            if (found != null)
            {
                return found.Command;
            }
            return null;
        }

        public CubaseMidiCommand GetMidiCommandByName(KnownCubaseMidiCommands name)
        {
            var commandName = this.GetCommandByName(name);  
            return this.GetCommandByCommand(commandName);
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

    public enum KnownCubaseMidiCommands
    {
        Rewind_To_Start,
        Start,
        Stop,
        Punch_In,
        Punch_Out,
        Set_Marker_1,
        Set_Marker_2,
        Go_To_Marker_1,
        Go_To_Marker_2,
        Show_Tracks_With_Data,
        Show_All_Tracks,
        Show_Selected_Tracks,
        Zoom_Selected_Tracks,
        Undo_Zoom,
        Record_Enable,
        Collapse_Tracks,
        Expand_Tracks,
        Mixer,
        Hide_Audio,
        Hide_Groups,
        Hide_Inputs,
        Hide_Instruments,
        Hide_Midi,
        Hide_Outputs,
        Mixer_Show_All,
        Next_Track,
        Previous_Track,
        // this is actually a mapped key command 
        Key_Hide_All,
        Key_Show_All
    }
}
