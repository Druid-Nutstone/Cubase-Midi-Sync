using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    public class CubaseMacroCommandCollection : List<CubaseMacroCommand>
    {
    }

    public class CubaseMacroCommand
    {
        public string Name { get; set; }    
    }
}
