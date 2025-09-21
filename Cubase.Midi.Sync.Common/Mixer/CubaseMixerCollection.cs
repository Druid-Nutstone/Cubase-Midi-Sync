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


        public CubaseMixerCollection(string keyCommandFileLocation)
        {
            this.cubaseMidiCommands = new CubaseMidiCommandCollection(keyCommandFileLocation);
        }

        public CubaseMixerCollection()
        {

        }

        public string ErrorMessage { get; set; }

        public bool Success { get; set; } = true;

        public CubaseMidiCommand GetMidiCommandByName(KnownCubaseMidiCommands name)
        {
            return cubaseMidiCommands.GetMidiCommandByName(name);
        }

        public CubaseMixer GetMixerCommand(KnownCubaseMidiCommands command)
        {
            return this.FirstOrDefault(x => x.Command == command);
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
                    if (!cubaseMixer.Toggled)
                    {
                        this.Toggle(KnownCubaseMidiCommands.Mixer);
                        // open mixer
                        commands.Add(cubaseMidiCommands.GetMidiCommandByName(cubaseMixer.Command));
                        commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Key_Show_All));


                        // hide all
                        //commands.AddRange(this.ToggleGroups());
                        this.GetMixerCommand(KnownCubaseMidiCommands.Show_Selected_Tracks).WithVisible(true).WithToggled(false);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Groups).WithToggled(false).WithVisible(true);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Audio).WithToggled(false).WithVisible(true);    
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Inputs).WithToggled(false).WithVisible(true);   
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Instruments).WithToggled(false).WithVisible(true);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Midi).WithToggled(false).WithVisible(true);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Outputs).WithToggled(false).WithVisible(true);  
                    }
                    else
                    {
                        this.Toggle(command);
                        if (!this.GetMixerCommand(KnownCubaseMidiCommands.Show_All_Tracks).Toggled)
                        {
                            commands = this.ToggleGroups();
                            this.GetMixerCommand(KnownCubaseMidiCommands.Show_Selected_Tracks).WithVisible(true).WithToggled(false);
                            this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Groups).WithToggled(false).WithVisible(false);
                            this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Audio).WithToggled(false).WithVisible(false);
                            this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Inputs).WithToggled(false).WithVisible(false);
                            this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Instruments).WithToggled(false).WithVisible(false);
                            this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Midi).WithToggled(false).WithVisible(false);
                            this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Outputs).WithToggled(false).WithVisible(false);
                        }
                        else
                        {
                            commands = this.ShowAll();  
                        }
                        commands.Add(cubaseMidiCommands.GetMidiCommandByName(cubaseMixer.Command));
                    }
                    return commands;
                case KnownCubaseMidiCommands.Show_All_Tracks:
                    if (!this.GetMixerCommand(KnownCubaseMidiCommands.Show_All_Tracks).Toggled)
                    {
                        commands = this.ToggleGroups();
                        this.GetMixerCommand(KnownCubaseMidiCommands.Show_Selected_Tracks).WithVisible(true).WithToggled(false);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Groups).WithToggled(false).WithVisible(false);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Audio).WithToggled(false).WithVisible(false);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Inputs).WithToggled(false).WithVisible(false);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Instruments).WithToggled(false).WithVisible(false);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Midi).WithToggled(false).WithVisible(false);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Outputs).WithToggled(false).WithVisible(false);
                    }
                    else
                    {
                        // hide all
                        commands = this.ToggleGroups();
                        this.GetMixerCommand(KnownCubaseMidiCommands.Show_Selected_Tracks).WithVisible(false).WithToggled(false);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Groups).WithToggled(false).WithVisible(true);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Audio).WithToggled(false).WithVisible(true);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Inputs).WithToggled(false).WithVisible(true);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Instruments).WithToggled(false).WithVisible(true);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Midi).WithToggled(false).WithVisible(true);
                        this.GetMixerCommand(KnownCubaseMidiCommands.Hide_Outputs).WithToggled(false).WithVisible(true);
                    }
                    this.Toggle(KnownCubaseMidiCommands.Show_All_Tracks);
                    return commands;
                case KnownCubaseMidiCommands.Show_Selected_Tracks:
                    if (cubaseMixer.Toggled)
                    {
                        commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Show_All_Tracks));
                    }
                    else
                    {
                        commands.Add(cubaseMidiCommands.GetMidiCommandByName(cubaseMixer.Command));
                    }
                    this.Toggle(KnownCubaseMidiCommands.Show_Selected_Tracks);
                    return commands;
                default:
                    if (cubaseMixer != null)
                    {
                        if (!cubaseMixer.Toggled && !areTracksHidden)
                        {
                            commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Key_Hide_All));
                            areTracksHidden = true;
                        }
                        else
                        {
                            areTracksHidden = false;
                        }
                        this.Toggle(cubaseMixer.Command);
                        
                        commands.Add(cubaseMidiCommands.GetMidiCommandByName(cubaseMixer.Command));
                        return commands;
                    }
                    return commands;
            }  
        }

        private List<CubaseMidiCommand> ToggleGroups()
        {
            var commands = new List<CubaseMidiCommand>();
            // hide all 
            if (this.ToggleIFNotToggled(KnownCubaseMidiCommands.Hide_Audio))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Audio));
            if (this.ToggleIFNotToggled(KnownCubaseMidiCommands.Hide_Groups))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Groups));
            if (this.ToggleIFNotToggled(KnownCubaseMidiCommands.Hide_Inputs))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Inputs));
            if (this.ToggleIFNotToggled(KnownCubaseMidiCommands.Hide_Instruments))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Instruments));
            if (this.ToggleIFNotToggled(KnownCubaseMidiCommands.Hide_Midi))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Midi));
            if (this.ToggleIFNotToggled(KnownCubaseMidiCommands.Hide_Outputs))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Outputs));
            return commands;    
        }

        private List<CubaseMidiCommand> ShowAll()
        {
            var commands = new List<CubaseMidiCommand>();
            if (this.ToggleIFToggled(KnownCubaseMidiCommands.Hide_Audio))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Audio));
            if (this.ToggleIFToggled(KnownCubaseMidiCommands.Hide_Groups))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Groups));
            if (this.ToggleIFToggled(KnownCubaseMidiCommands.Hide_Inputs))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Inputs));
            if (this.ToggleIFToggled(KnownCubaseMidiCommands.Hide_Instruments))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Instruments));
            if (this.ToggleIFToggled(KnownCubaseMidiCommands.Hide_Midi))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Midi));
            if (this.ToggleIFToggled(KnownCubaseMidiCommands.Hide_Outputs))
                commands.Add(cubaseMidiCommands.GetMidiCommandByName(KnownCubaseMidiCommands.Hide_Outputs));
            return commands;
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
              CubaseMixer.Create(KnownCubaseMidiCommands.Show_Selected_Tracks, "Show Selected Tracks", "Undo Selected Tracks").WithInitiallyVisible(false),
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

        public bool IsInitiallyVisible { get; set; } = true;

        public CubaseButtonType ButtonType { get; set; } = CubaseButtonType.MacroToggle;

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
                Visible = visible,
                ButtonType = CubaseButtonType.MacroToggle
            };
        }
    }
}
