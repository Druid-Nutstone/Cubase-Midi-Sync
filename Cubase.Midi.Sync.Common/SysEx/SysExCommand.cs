using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.SysEx
{
    public enum SysExCommand
    {
         DisableRecord = 0,
         EnableRecord = 1,
         DisableAndEnable = 2,
         SelectTracks = 3,
         EnableMute = 4,
         DisableMute = 5,
         EnableSolo = 6,
         DisableSolo = 7,
         EnableListen = 8,
         DisableListen = 9
    }
}
