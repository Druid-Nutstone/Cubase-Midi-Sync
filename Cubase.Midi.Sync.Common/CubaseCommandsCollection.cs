using Cubase.Midi.Sync.Common.Colours;
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

        public CubaseCommandCollection WithNewCubaseCommand(string name, string category)         {
            var collection = new CubaseCommandCollection() { Name = name, Category = category };
            this.Add(collection);
            return collection;
        }

        public List<string> GetNames()
        {
            return this.Select(x=> x.Name).ToList();    

        }

        public bool HaveName(string name)
        {
            return this.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));   
        }

        public CubaseCommandCollection GetCommandCollectionByName(string name)
        {
            return this.First(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }


    public class CubaseCommandCollection
    {
        public string Name { get; set; }    
    
        public string Category { get; set; }

        public SerializableColour BackgroundColour { get; set; } = ColourConstants.ButtonBackground.ToSerializableColour();
        
        public SerializableColour TextColour { get; set; }  = ColourConstants.ButtonText.ToSerializableColour();    

        public List<CubaseCommand> Commands { get; set; } = new List<CubaseCommand>();

        public CubaseCommandCollection WithNewCubaseCommand(CubaseCommand cubaseCommand)
        {
            Commands.Add(cubaseCommand.WithCategory(this.Category));
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
    }

    public class CubaseCommand
    {
        public string Name { get; set; }

        public string Action { get; set; }

        public List<string> ActionGroup { get; set; } = new List<string>();

        public string Category { get; set; }

        public SerializableColour ToggleBackGroundColour { get; set; } = ColourConstants.ButtonBackground.ToSerializableColour();

        public SerializableColour ToggleForeColour { get; set; } = ColourConstants.ButtonText.ToSerializableColour();

        public CubaseButtonType ButtonType { get; set; } = CubaseButtonType.Momentory;


        public bool IsToggled { get; set; }

        public SerializableColour ButtonBackgroundColour { get; set; }

        public SerializableColour ButtonTextColour { get; set; }

        public SerializableColour ButtonColour => (ButtonType == CubaseButtonType.Toggle && IsToggled)
            ? this.ToggleBackGroundColour
            : ButtonBackgroundColour;

        public SerializableColour TextColor => (ButtonType == CubaseButtonType.Toggle && IsToggled)
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

        public CubaseCommand WithButtonTextColour(Color color)
        {
            this.ButtonTextColour = color.ToSerializableColour();   
            return this;
        }

        public CubaseCommand WithCategory(string category)
        {
            this.Category = category;
            return this;
        }

        public static CubaseCommand Create()
        {
            return new CubaseCommand()
            {
                ButtonBackgroundColour = ColourConstants.ButtonBackground.ToSerializableColour(),
                ButtonTextColour = ColourConstants.ButtonText.ToSerializableColour(),
                ToggleBackGroundColour = ColourConstants.ButtonToggledBackground.ToSerializableColour(),
                ToggleForeColour = ColourConstants.ButtonToggledText.ToSerializableColour()
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
                ButtonTextColour = ColourConstants.ButtonText.ToSerializableColour()
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
                ButtonTextColour = ColourConstants.ButtonText.ToSerializableColour()
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
                ToggleForeColour = ColourConstants.ButtonToggledText.ToSerializableColour()
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
                ToggleForeColour = ColourConstants.ButtonToggledText.ToSerializableColour()
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
    }
}
