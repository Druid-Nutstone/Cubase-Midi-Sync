using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Requests
{
    public class CubaseActionRequest
    {
        public CubaseActionName Action { get; set; }

        public CubaseAreaName Area { get; set; }

        public static CubaseActionRequest Create(CubaseActionName action)
        {
            return new CubaseActionRequest() { Action = action };
        }

        public static CubaseActionRequest CreateFromCommand(CubaseCommand command)
        {
            return new CubaseActionRequest() { Action = command.Action };
        }
    }
}
