using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Cubase.Midi.Sync.Common.Midi;

namespace Cubase.Midi.Sync.Server.Tests.Tests.Tracks
{
    [TestClass]
    public class TrackTests : BaseTest
    {
         [TestMethod]
        public async Task CanGetTracks()
        {
            Thread.Sleep(10000);
            
            var tracks = await this.Client.GetFromJsonAsync<MidiChannelCollection>("api/cubase/tracks");
            var activeChannels = tracks.GetActiveChannels();    
        }
    }
}
