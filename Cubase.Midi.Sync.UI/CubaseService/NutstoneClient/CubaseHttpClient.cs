using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.UI.NutstoneServices.NutstoneClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.CubaseService.NutstoneClient
{
    public class CubaseHttpClient : HttpClient, ICubaseHttpClient
    {
        public async Task<CubaseCommandsCollection> GetCommands()
        {
            throw new NotImplementedException();
        }
    }
}
