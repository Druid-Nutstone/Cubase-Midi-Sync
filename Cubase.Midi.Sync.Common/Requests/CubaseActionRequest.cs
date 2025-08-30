using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Requests
{
    public class CubaseActionRequest
    {
        public string Action { get; set; }

        public List<string> ActionGroup { get; set; }

        public string Category { get; set; }

        public CubaseButtonType ButtonType { get; set; }

        public static CubaseActionRequest Create(string action)
        {
            return new CubaseActionRequest() { Action = action };
        }

        public static CubaseActionRequest CreateFromCommand(CubaseCommand command)
        {
            if (command.ButtonType == CubaseButtonType.Macro)
            {
                return new CubaseActionRequest()
                {
                    ActionGroup = command.ActionGroup,
                    Category = command.Category,
                    ButtonType = command.ButtonType,
                };
            }
            else
            {
                return new CubaseActionRequest()
                {
                    Action = command.Action,
                    Category = command.Category,
                    ButtonType = command.ButtonType,    
                };
            }
        }
    }
}
