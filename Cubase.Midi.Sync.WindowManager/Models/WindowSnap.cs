using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.WindowManager.Models
{
    public class WindowSnap
    {
        public string WindowName { get; set; }  
        
        public string Name { get; set; }    
        
        public WindowPositionCollection Positions { get; set; }

    }
}
