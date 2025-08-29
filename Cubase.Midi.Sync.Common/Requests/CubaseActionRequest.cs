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

        public string Category { get; set; }

        public static CubaseActionRequest Create(string action)
        {
            return new CubaseActionRequest() { Action = action };
        }

        public static CubaseActionRequest CreateFromCommand(CubaseCommand command)
        {
            return new CubaseActionRequest() { Action = command.Action, Category = command.Category };
        }
    }
}
