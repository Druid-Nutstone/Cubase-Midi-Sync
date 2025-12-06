using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Scripts
{
    public enum ScriptFunction
    {
        SelectTrack = 0,
        GetTracks = 1,
        DisableRecord = 2,
        EnableRecord = 3,
        ExecuteCommand = 4,
        Unknown = 99
    }
}
