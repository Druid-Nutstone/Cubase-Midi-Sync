using Cubase.Midi.Sync.Common.Keys;
using Cubase.Midi.Sync.Common.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Mixer
{
    public class CubaseMixerCollection : List<CubaseMixer>
    {
        private CubaseMidiCommandCollection cubaseMidiCommands;
        
        private bool areTracksHidden = false;   

        public List<string> MixerConsoleNames = new List<string>();

        public CubaseMixerCollection(string keyCommandFileLocation)
        {
            this.cubaseMidiCommands = new CubaseMidiCommandCollection(keyCommandFileLocation);
        }

        public CubaseMixerCollection()
        {

        }

        public bool IsShowSelectedTracks { get; set; } = false; 

        public string ErrorMessage { get; set; }

        public bool Success { get; set; } = true;

        public CubaseMidiCommand GetMidiCommandByName(KnownCubaseMidiCommands name)
        {
            return cubaseMidiCommands.GetMidiCommandByName(name);
        }

        public List<CubaseMixer> GetStaticMixConsoleCommands()
        {
            return new List<CubaseMixer>() {
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Groups, "Show Groups"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Audio, "Show Audio"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Instruments, "Show Intruments"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Midi, "Show Midi"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Inputs, "Show Inputs"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Outputs, "Show Outputs"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Show_All_Tracks, "Show All Tracks"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Key_Show_Selected, "Show Selected Tracks"),
            };
        }

        public CubaseMixer GetMixerCommand(KnownCubaseMidiCommands command)
        {
            return this.FirstOrDefault(x => x.Command == command);
        }



        public CubaseMixerCollection WithMixerNames(List<string> mixerConsoleName)
        {
            this.MixerConsoleNames = mixerConsoleName;
            return this;
        }

        public CubaseMixerCollection Toggle(KnownCubaseMidiCommands command)
        {
            var cmd = this.GetMixerCommand(command);
            cmd.Toggled = !cmd.Toggled;
            return this;
        }

        public CubaseMixerCollection ToggleVisibility(KnownCubaseMidiCommands command, bool visible)
        {
            this.GetMixerCommand(command).Visible = visible;    
            return this;
        }

        public bool ToggleIFNotToggled(KnownCubaseMidiCommands command)
        {
            var mixer = this.GetMixerCommand(command);
            if (mixer != null && !mixer.Toggled)
            {
                mixer.Toggled = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ToggleIFToggled(KnownCubaseMidiCommands command)
        {
            var mixer = this.GetMixerCommand(command);
            if (mixer != null && mixer.Toggled)
            {
                mixer.Toggled = false;
                return true;
            }
            return false;
        }

        public CubaseMixer GetMixerByCommand(KnownCubaseMidiCommands command)
        {
            return this.FirstOrDefault(x => x.Command == command);
        } 
        
        public List<CubaseMidiCommand> GetCommands(KnownCubaseMidiCommands command)
        {
            var commands = new List<CubaseMidiCommand>();
            var cubaseMixer = this.GetMixerCommand(command);
            switch (cubaseMixer.Command)
            {
                case KnownCubaseMidiCommands.Mixer:
                    return commands;
                case KnownCubaseMidiCommands.Show_Selected_Tracks:
                    commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Key_Show_Selected));
                //    this.IsShowSelectedTracks = true;
                    return commands;
                case KnownCubaseMidiCommands.Show_All_Tracks:
                    //if (this.IsShowSelectedTracks)
                    //{
                    //    this.IsShowSelectedTracks = false;
                    //    commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Show_All_Tracks));
                    //}
                   commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Key_Show_All));
                   return commands;
                default:
                    commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Key_Hide_All));
                    commands.Add(cubaseMidiCommands.GetMidiCommandByName(cubaseMixer.Command));
                    return commands;

            }  
        }


        public static CubaseMixerCollection Create(Action<string> msgHandler, string keyCommandFileLocation)
        {
            // get key commands 
            var requiredKeys = RequiredKeyMappingCollection.Create(msgHandler, keyCommandFileLocation);
            return new CubaseMixerCollection(keyCommandFileLocation)
            {
              CubaseMixer.Create(KnownCubaseMidiCommands.Mixer, "Open Mixer", "Close Mixer", false, false),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Groups, "Show Groups", "Hide Groups"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Audio, "Show Audio", "Hide Audio" ),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Instruments, "Show Intruments", "Hide Instruments"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Midi, "Show Midi", "Hide Midi"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Inputs, "Show Inputs", "Hide Inputs"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Hide_Outputs, "Show Outputs", "Hide Outputs"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Show_All_Tracks, "Show All Tracks", "Hide Tracks"),
              CubaseMixer.Create(KnownCubaseMidiCommands.Key_Show_Selected, "Show Selected Tracks", "Hide Selected Tracks"),
              CubaseMixer.CreateKey(KnownCubaseMidiCommands.Key_Hide_All, requiredKeys.GetKey(RequiredKeyId.Mixer_Hide_All), "Hide Channel Types", "NA", false, false),
              CubaseMixer.CreateKey(KnownCubaseMidiCommands.Key_Show_All, requiredKeys.GetKey(RequiredKeyId.Mixer_Show_All), "Show Channel Types", "NA", false, false)
            };
        }
    }


    public class CubaseMixer
    {
        public KnownCubaseMidiCommands Command { get; set; }

        public string KeyAction { get; set; }
        
        public bool Toggled { get; set; }

        public bool Visible { get; set; }

        public string MixerConsoleName { get; set; }    

        public bool IsInitiallyVisible { get; set; } = true;

        public string ButtonText { get; set; }  
        

        public string ButtonTextToggled { get; set; }   
        
        public KnownCubaseMidiCommands CommandToggled { get; set; } 

        public CubaseMixer WithToggled(bool toggled)
        {
            this.Toggled = toggled; 
            return this;
        }   
        
        public CubaseMixer WithVisible(bool visible)
        {
            this.Visible = visible; 
            return this;
        }

        public CubaseMixer WithInitiallyVisible(bool visible)
        {
            this.IsInitiallyVisible = visible;
            return this;
        }

        public CubaseMixer WithMixerConsoleName(string mixerConsoleName)
        {
            this.MixerConsoleName = mixerConsoleName;
            return this;
        }

        public static CubaseMixer Create(KnownCubaseMidiCommands command, string buttonText)
        {
            return new CubaseMixer
            {
                Command = command,
                ButtonText = buttonText,
            };
        }

        public static CubaseMixer CreateKey(KnownCubaseMidiCommands command, string keyValue, string buttonText, string buttonTextToggled, bool toggled = false, bool visible = true)
        {
            return new CubaseMixer
            {
                Command = command,
                KeyAction = keyValue,
                ButtonText = buttonText,
                ButtonTextToggled = buttonTextToggled,
                Toggled = toggled,

                Visible = visible
            };
        }

        public static CubaseMixer Create(KnownCubaseMidiCommands command, string buttonText, string buttonTextToggled, bool toggled = false, bool visible = true)
        {
            return new CubaseMixer
            {
                Command = command,
                ButtonText = buttonText,    
                ButtonTextToggled = buttonTextToggled,
                Toggled = toggled,
                Visible = visible
            };
        }

        public static CubaseMixer CreateMacroToggle(KnownCubaseMidiCommands command, KnownCubaseMidiCommands commandToggled, string buttonText, string buttonTextToggled, bool toggled = false, bool visible = true)
        {
            return new CubaseMixer
            {
                Command = command,
                CommandToggled = commandToggled,
                ButtonText = buttonText,
                ButtonTextToggled = buttonTextToggled,
                Toggled = toggled,
                Visible = visible
            };
        }
    }
}
