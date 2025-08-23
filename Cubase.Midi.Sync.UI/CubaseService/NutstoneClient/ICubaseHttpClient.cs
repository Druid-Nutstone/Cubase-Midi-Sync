using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient
{
    public interface ICubaseHttpClient
    {
        Task<CubaseCommandsCollection> GetCommands();
    }
}
