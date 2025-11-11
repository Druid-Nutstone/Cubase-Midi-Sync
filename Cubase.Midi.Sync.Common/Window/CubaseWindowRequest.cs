using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Window
{
    public class CubaseWindowRequest
    {
        public CubaseWindowRequestCommand Command { get; set; } 
        public CubaseWindowRequest() { }
        public static CubaseWindowRequest CreateCommand(CubaseWindowRequestCommand command)
        {
            return new CubaseWindowRequest() { Command = command }; 
        }
    
    }

    public enum CubaseWindowRequestCommand
    {
        ActiveWindows = 0,
    }
}
