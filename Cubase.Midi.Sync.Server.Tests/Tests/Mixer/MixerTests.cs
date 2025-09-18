using Cubase.Midi.Sync.Common.Mixer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Server.Tests.Tests.Mixer
{
    [TestClass]
    public class MixerTests : BaseTest
    {
        [TestMethod]
        public async Task Can_Open_Mixer()
        {             
            Thread.Sleep(10000);

            //var cubaseMixer = CubaseMixer.Create(Common.Midi.KnownCubaseMidiCommands.Mixer);
            //var cubaseAudio = CubaseMixer.Create(Common.Midi.KnownCubaseMidiCommands.Hide_Audio);

            //var response = await this.Client.PostAsJsonAsync<CubaseMixer>("api/mixer", cubaseMixer);

            //var audoResponse = await this.Client.PostAsJsonAsync<CubaseMixer>("api/mixer", cubaseAudio);

            //var response2 = await this.Client.PostAsJsonAsync<CubaseMixer>("api/mixer", cubaseMixer);   

        }
    }
}
