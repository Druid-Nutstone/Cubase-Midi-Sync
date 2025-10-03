using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Requests
{
    public class CubaseActionRequest
    {
        public ActionEvent Action { get; set; }

        public List<ActionEvent> ActionGroup { get; set; }

        public CubaseButtonType ButtonType { get; set; }

        public bool IsMacro()
        {
            return ButtonType == CubaseButtonType.Macro || ButtonType == CubaseButtonType.MacroToggle;
        }

        public static CubaseActionRequest CreateSingleMidiCommand(MidiAndKey midiAndKey)
        {
            return new CubaseActionRequest() { Action = ActionEvent.Create(midiAndKey.KeyType, midiAndKey.Action), ButtonType = CubaseButtonType.Momentory };
        }


        public static CubaseActionRequest CreateFromCommand(CubaseCommand command, List<ActionEvent>? actionGroup = null)
        {
            switch (command.ButtonType)
            {
                case CubaseButtonType.Macro:
                case CubaseButtonType.MacroToggle:
                    return new CubaseActionRequest()
                    {
                        ActionGroup = actionGroup != null ? actionGroup : (command.IsToggled ? command.ActionGroup : command.ActionGroupToggleOff),
                        ButtonType = command.ButtonType,
                    };
                default:
                    return new CubaseActionRequest()
                    {
                        Action = command.Action,
                        ButtonType = command.ButtonType,
                    };
            }
        }
    }
}
