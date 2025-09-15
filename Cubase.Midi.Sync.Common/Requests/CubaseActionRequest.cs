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

        public bool IsMacro()
        {
            return ButtonType == CubaseButtonType.Macro || ButtonType == CubaseButtonType.MacroToggle;
        }

        public static CubaseActionRequest CreateSingleMidiCommand(string action)
        {
            return new CubaseActionRequest() { Action = action, ButtonType = CubaseButtonType.Momentory, Category = CubaseServiceConstants.MidiService };
        }

        public static CubaseActionRequest Create(string action)
        {
            return new CubaseActionRequest() { Action = action };
        }

        public static CubaseActionRequest CreateMidiActionGroup(string[] actions)
        {
            return new CubaseActionRequest() { ActionGroup = actions.ToList(), ButtonType = CubaseButtonType.Macro, Category = CubaseServiceConstants.MidiService };
        }

        public static CubaseActionRequest CreateFromCommand(CubaseCommand command, List<string>? actionGroup = null)
        {
            switch (command.ButtonType)
            {
                case CubaseButtonType.Macro:
                case CubaseButtonType.MacroToggle:
                    return new CubaseActionRequest()
                    {
                        ActionGroup = actionGroup != null ? actionGroup : (command.IsToggled ? command.ActionGroup : command.ActionGroupToggleOff),
                        Category = command.Category,
                        ButtonType = command.ButtonType,
                    };
                default:
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
