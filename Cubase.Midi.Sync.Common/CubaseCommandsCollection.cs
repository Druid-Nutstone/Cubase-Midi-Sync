﻿using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;


namespace Cubase.Midi.Sync.Common
{
    public class CubaseCommandsCollection : List<CubaseCommandCollection>
    {

        public CubaseCommandsCollection() { }

        public CubaseCommandsCollection(List<CubaseCommandCollection> commands)
        {
            this.AddRange(commands);
        }
        
        public bool HaveError { get; set; } = false;

        public string Message { get; set; }

        public void SaveToFile(string fileName)
        {
            var asText = JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(fileName, asText);
        }

        public static CubaseCommandsCollection CreateWithError(string message)
        {
            return new CubaseCommandsCollection() { HaveError = true, Message = message };
        }

        public static CubaseCommandsCollection LoadFromFile(string fileName)
        {
            return JsonSerializer.Deserialize<CubaseCommandsCollection>(File.ReadAllText(fileName));
        }

        public CubaseCommandCollection WithNewCubaseCommand(string name, string category)
        {
            var collection = new CubaseCommandCollection() { Name = name, Category = category };
            this.Add(collection);
            return collection;
        }

        public List<string> GetNames()
        {
            return this.Select(x => x.Name).ToList();

        }

        public bool HaveName(string name)
        {
            return this.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public CubaseCommandCollection GetCommandCollectionByName(string name)
        {
            if (this.Any())
            {
                return this.First(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
            return null;
        }

        public bool RemoveCubaseCommand(CubaseCommand command, bool fromAllInstances = false)
        {
            foreach (var commandColl in this)
            {
                for (int i=0; i < commandColl.Commands.Count; i++)
                {
                    if (commandColl.Commands[i].Name == command.Name)
                    {
                        if (fromAllInstances)
                        {
                            commandColl.Commands.RemoveAt(i);
                        }
                        else
                        {
                            if (commandColl.Name == command.ParentCollectionName)
                            {
                                commandColl.Commands.RemoveAt(i);
                            }
                        }
                    }
                }  
            }
            // clean up any empty collections 
            for (int i=0; i < this.Count; i++)
            {
                if (this[i].Commands.Count == 0)
                {
                    this.RemoveAt(i);   
                }
            }
            return true;
        }

        public static CubaseCommandsCollection CreateFromList(List<CubaseCommandCollection> commands)
        {
            return new CubaseCommandsCollection(commands);
        }
    }

    public class CubaseCommandCollection
    {
        public string Name { get; set; }    
    
        public string Category { get; set; }

        public bool Visible { get; set; } = true;

        public SerializableColour BackgroundColour { get; set; } = ColourConstants.ButtonBackground.ToSerializableColour();
        
        public SerializableColour TextColour { get; set; }  = ColourConstants.ButtonText.ToSerializableColour();    

        public List<CubaseCommand> Commands { get; set; } = new List<CubaseCommand>();

        public CubaseCommandCollection WithNewCubaseCommand(CubaseCommand cubaseCommand)
        {
            Commands.Add(cubaseCommand.WithParentCollectionName(this.Name));
            return this;
        }

        public CubaseCommandCollection WithBackgroundColour(SerializableColour backgroundColour) 
        { 
            this.BackgroundColour = backgroundColour;
            return this;
        }

        public CubaseCommandCollection WithTextColour(SerializableColour textColour)
        {
            this.TextColour = textColour;
            return this;
        }

        public CubaseCommandCollection WithCategory(string category)
        {
            this.Category = category;
            return this;
        }
    }

    public class CubaseCommand
    {
        public string Name { get; set; }

        // button text when toggled
        public string NameToggle { get; set; }
        
        public string ParentCollectionName { get; set; }

        public string Action { get; set; }

        public List<string> ActionGroup { get; set; } = new List<string>();

        public List<string> ActionGroupToggleOff { get; set; } = new List<string>();  

        public string Category { get; set; }

        public SerializableColour ToggleBackGroundColour { get; set; } = ColourConstants.ButtonBackground.ToSerializableColour();

        public SerializableColour ToggleForeColour { get; set; } = ColourConstants.ButtonText.ToSerializableColour();

        public CubaseButtonType ButtonType { get; set; } = CubaseButtonType.Momentory;

        public CubaseKnownCommand CubaseCommandDefinition { get; set; }   

        public DateTime? Created { get; set; }   
        
        public bool IsMacro 
        { 
            get
            {
                return this.ButtonType == CubaseButtonType.Macro || this.ButtonType == CubaseButtonType.MacroToggle;
            } 
        }

        public bool IsToggleButton
        {
            get
            {
                return this.ButtonType == CubaseButtonType.Toggle || this.ButtonType == CubaseButtonType.MacroToggle;
            }
        } 

        public bool IsToggled { get; set; }

        public SerializableColour ButtonBackgroundColour { get; set; }

        public SerializableColour ButtonTextColour { get; set; }

        public SerializableColour ButtonColour => (IsToggleButton && IsToggled)
            ? this.ToggleBackGroundColour
            : ButtonBackgroundColour;

        public SerializableColour TextColor => (IsToggleButton && IsToggled)
            ? this.ToggleForeColour
            : ButtonTextColour;
    

        
        public CubaseCommand WithButtonType(CubaseButtonType buttonType)
        {
            this.ButtonType = buttonType;
            return this;
        }   

        public CubaseCommand WithToggleBackGroundColour(Color colour)
        {
            this.ToggleBackGroundColour = colour.ToSerializableColour();
            return this;
        }

        public CubaseCommand WithToggleForeColour(Color colour)
        {
            this.ToggleForeColour = colour.ToSerializableColour();
            return this;
        }

        public CubaseCommand WithButtonBackgroundColour(Color colour)
        {
            this.ButtonBackgroundColour = colour.ToSerializableColour();
            return this;
        }

        public CubaseCommand WithNameToggle(string nameToggle)
        {
            this.NameToggle = nameToggle;   
            return this;
        }

        public CubaseCommand WithButtonTextColour(Color color)
        {
            this.ButtonTextColour = color.ToSerializableColour();   
            return this;
        }

        public CubaseCommand WithParentCollectionName(string name)
        {
            this.ParentCollectionName = name;
            return this;
        }

        public CubaseCommand WithCategory(string category)
        {
            this.Category = category;
            return this;
        }

        public CubaseCommand WithName(string name)
        {
            this.Name = name;
            return this;
        }

        public CubaseCommand WithAction(string action)
        {
            this.Action = action;
            return this;
        }

        public CubaseCommand WithActionGroup(List<string> actionGroups)
        {
            this.ActionGroup = actionGroups;
            return this;
        }

        public CubaseCommand WithActionGroupToggleOff(List<string> actionGroupsOff)
        {
            this.ActionGroupToggleOff = actionGroupsOff;
            return this;
        }

        public static CubaseCommand Create()
        {
            return new CubaseCommand()
            {
                ButtonBackgroundColour = ColourConstants.ButtonBackground.ToSerializableColour(),
                ButtonTextColour = ColourConstants.ButtonText.ToSerializableColour(),
                ToggleBackGroundColour = ColourConstants.ButtonToggledBackground.ToSerializableColour(),
                ToggleForeColour = ColourConstants.ButtonToggledText.ToSerializableColour(),
                Created = DateTime.Now
            };
        }

        public static CubaseCommand CreateMacroToggleButton(string name, List<string> actions, List<string> actionsToggleOff)
        {
            return new CubaseCommand()
            {
                Name = name,
                ActionGroup = actions,
                ActionGroupToggleOff= actionsToggleOff,
                ButtonType = CubaseButtonType.MacroToggle,
                ButtonBackgroundColour = ColourConstants.ButtonBackground.ToSerializableColour(),
                ButtonTextColour = ColourConstants.ButtonText.ToSerializableColour(),
                Created = DateTime.Now,
            };
        }

        public static CubaseCommand Create(string name, string action)
        {
            return new CubaseCommand()
            {
                Name = name,
                Action = action,
                ButtonType = CubaseButtonType.Momentory,
                ButtonBackgroundColour = ColourConstants.ButtonBackground.ToSerializableColour(),
                ButtonTextColour = ColourConstants.ButtonText.ToSerializableColour(),
                Created = DateTime.Now,
            };
        }

        public static CubaseCommand CreateStandardButton(string name, string action)
        {
            return new CubaseCommand()
            {
                Name = name,
                Action = action,
                ButtonType = CubaseButtonType.Momentory,
                ButtonBackgroundColour = ColourConstants.ButtonBackground.ToSerializableColour(),
                ButtonTextColour = ColourConstants.ButtonText.ToSerializableColour(),
                Created = DateTime.Now,
            };
        }

        public static CubaseCommand CreateMacroButton(string name, List<string> actions)
        {
            return new CubaseCommand()
            {
                Name = name,
                ActionGroup = actions,
                ButtonType = CubaseButtonType.Macro, 
                ButtonBackgroundColour = ColourConstants.ButtonBackground.ToSerializableColour(),
                ButtonTextColour = ColourConstants.ButtonText.ToSerializableColour(),
                ToggleBackGroundColour = ColourConstants.ButtonToggledBackground.ToSerializableColour(),
                ToggleForeColour = ColourConstants.ButtonToggledText.ToSerializableColour(),
                Created = DateTime.Now
            };
        }

        public static CubaseCommand CreateToggleButton(string name, string action)
        {
            return new CubaseCommand()
            {
                Name = name,
                Action = action,
                ButtonType = CubaseButtonType.Toggle,
                ButtonBackgroundColour = ColourConstants.ButtonBackground.ToSerializableColour(),
                ButtonTextColour = ColourConstants.ButtonText.ToSerializableColour(),
                ToggleBackGroundColour = ColourConstants.ButtonToggledBackground.ToSerializableColour(),
                ToggleForeColour = ColourConstants.ButtonToggledText.ToSerializableColour(),
                Created = DateTime.Now
            };
        }

    }

    public enum CubaseButtonType
    {
        /// <summary>
        /// the action can be toggled on and off
        /// </summary>
        Toggle,
        /// <summary>
        /// Represents a momentary state or condition.
        /// </summary>
        /// <remarks>This class or member is intended to encapsulate a transient or temporary state. 
        /// Additional context or functionality should be provided to clarify its specific use case.</remarks>
        Momentory,
        /// <summary>
        /// More that one command 
        /// </summary>
        Macro,
        MacroToggle,
    }
}
