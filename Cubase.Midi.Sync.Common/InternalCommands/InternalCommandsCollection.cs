using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.InternalCommands
{
    public class InternalCommandsCollection : List<InternalCommand> 
    {
        public static string CommandIdentifier = "%";
        
        public InternalCommandsCollection() 
        {
            this.Add(InternalCommand.Create(InternalCommandType.Navigate));
        }

        public static bool IsInternalCommand(string command)
        {
            return command.StartsWith(CommandIdentifier);
        }

        public static string SerialiseCommand(InternalCommand command)
        {
            return $"{CommandIdentifier}{Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(JsonSerializer.Serialize(command)))}{CommandIdentifier}";
        }
        
        public static InternalCommand DeserialiseCommand(string command)
        {
            if (!IsInternalCommand(command))
            {
                throw new ArgumentException("Not an internal command", nameof(command));
            }
            var base64 = command.Trim(CommandIdentifier.ToCharArray());
            var json = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            return JsonSerializer.Deserialize<InternalCommand>(json);
        }
    }

    public class InternalCommand
    {
        public InternalCommandType CommandType { get; set; }    

        public string Parameter { get; set; }
    
        public static InternalCommand Create(InternalCommandType commandType)
        {
            return new InternalCommand()
            {
                CommandType = commandType,
            };
        }
        public static InternalCommand Create(InternalCommandType commandType, string parameter)
        {
            return new InternalCommand()
            {
                CommandType = commandType,
                Parameter = parameter   
            };
        }
    }

    public enum InternalCommandType
    {
        Navigate
    }
}
