using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common
{
    public class CubaseCommandsCollection : List<CubaseCommandCollection>
    {
    }


    public class CubaseCommandCollection
    {
        public string Name { get; set; }    
    
        public List<CubaseCommand> Commands { get; set; } = new List<CubaseCommand>();  
    
    }

    public class CubaseCommand
    {
        public string Name { get; set; }    
    
        public string Action { get; set; }

    }
}
