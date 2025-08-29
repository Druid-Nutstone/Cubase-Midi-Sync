using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


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


    }


    public class CubaseCommandCollection
    {
        public string Name { get; set; }    
    
        public string Category { get; set; }

        public List<CubaseCommand> Commands { get; set; } = new List<CubaseCommand>();

        public CubaseCommandCollection WithNewCubaseCommand(CubaseCommand cubaseCommand)
        {
            Commands.Add(cubaseCommand.WithCategory(this.Category));
            return this;
        }

    }

    public class CubaseCommand
    {
        public string Name { get; set; }    
    
        public string Action { get; set; }

        public string Category { get; set; }

        public SerializableColour ToggleBackGroundColour { get; set; } = ColourConstants.ButtonBackground.ToSerializableColour();

        public SerializableColour ToggleForeColour { get; set; } = ColourConstants.ButtonText.ToSerializableColour();

        public CubaseButtonType ButtonType { get; set; } = CubaseButtonType.Momentory;


        public bool IsToggled { get; set; }

        //public Color ButtonColor => (ButtonType == CubaseButtonType.Toggle && IsToggled)
        //    ? this.ToggleBackGroundColour
        //    : ColourConstants.ButtonBackground;

        public SerializableColour ForeColor => (ButtonType == CubaseButtonType.Toggle && IsToggled)
            ? this.ToggleForeColour
            : ColourConstants.ButtonText.ToSerializableColour();
    
        public SerializableColour GetBackgroundColour()
        {
            if (ButtonType == CubaseButtonType.Toggle && IsToggled)
            {
                return this.ToggleBackGroundColour;
            }
            return ColourConstants.ButtonBackground.ToSerializableColour();
        }
        
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

        public CubaseCommand WithCategory(string category)
        {
            this.Category = category;
            return this;
        }

        public static CubaseCommand Create(string name, string action)
        {
            return new CubaseCommand()
            {
                Name = name,
                Action = action,
                ButtonType = CubaseButtonType.Momentory,
                ToggleBackGroundColour = ColourConstants.ButtonBackground.ToSerializableColour(),
                ToggleForeColour = ColourConstants.ButtonText.ToSerializableColour()
            };
        }

        public static CubaseCommand CreateStandardButton(string name, string action)
        {
            return new CubaseCommand()
            {
                Name = name,
                Action = action,
                ButtonType = CubaseButtonType.Momentory,
                ToggleBackGroundColour = ColourConstants.ButtonBackground.ToSerializableColour(),
                ToggleForeColour = ColourConstants.ButtonText.ToSerializableColour()
            };
        }

        public static CubaseCommand CreateToggleButton(string name, string action)
        {
            return new CubaseCommand()
            {
                Name = name,
                Action = action,
                ButtonType = CubaseButtonType.Toggle,
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
        Momentory
    }
}
