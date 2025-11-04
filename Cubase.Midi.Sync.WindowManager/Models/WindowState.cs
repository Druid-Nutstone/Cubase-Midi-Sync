using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.WindowManager.Models
{
    public enum WindowState
    {
        Hide = 0,
        Normal = 1,
        Minimized = 2,
        Maximized = 3,
        Restore = 9,
        Unknown = 99 
    }
}
