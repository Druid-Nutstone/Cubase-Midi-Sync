using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Responses
{
    public class CubaseActionResponse
    {
        public bool Success { get; set; } = true;  

        public string Message { get; set; }

        public static CubaseActionResponse CreateSuccess()
        {
            return new CubaseActionResponse() { Success = true };
        }
        public static CubaseActionResponse CreateError(string message)
        {
            return new CubaseActionResponse() { Success = false, Message = message };
        }
    }
}
