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

        public CubaseCommandCollection WithNewCubaseCommand(string name, CubaseAreaName cubaseAreaName)         {
            var collection = new CubaseCommandCollection() { Name = name, Area = cubaseAreaName };
            this.Add(collection);
            return collection;
        }


    }


    public class CubaseCommandCollection
    {
        public string Name { get; set; }    
    
        public CubaseAreaName Area { get; set; }

        public List<CubaseCommand> Commands { get; set; } = new List<CubaseCommand>();  
    
        public CubaseCommandCollection WithNewCubaseCommand(string name, CubaseActionName action, CubaseButtonType buttonType = CubaseButtonType.Momentory)
        {
            var command = new CubaseCommand() { Name = name, Action = action, ButtonType = buttonType, Area = this.Area};
            Commands.Add(command);
            return this;
        }

    }

    public class CubaseCommand
    {
        public string Name { get; set; }    
    
        public CubaseActionName Action { get; set; }

        public CubaseAreaName Area { get; set; }    


        public CubaseButtonType ButtonType { get; set; } = CubaseButtonType.Momentory;


        public bool IsToggled { get; set; }

        public Color ButtonColor => (ButtonType == CubaseButtonType.Toggle && IsToggled)
            ? Color.Green
            : Color.SkyBlue;

        public Color ForeColor => (ButtonType == CubaseButtonType.Toggle && IsToggled)
            ? Color.Black
            : Color.White;
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
