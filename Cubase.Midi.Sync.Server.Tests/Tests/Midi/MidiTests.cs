using Cubase.Midi.Sync.Server.Services.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Server.Tests.Tests.Midi
{
    [TestClass]
    public class MidiTests
    {
        [TestMethod]
        public void Can_Send_play()
        {
            NutstoneDriver nutstoneDriver = new NutstoneDriver("Nutstone");
            Thread.Sleep(2000);
            
            // send start 
            nutstoneDriver.SendNoteOn(0, 2, 127);
        }
    }
}
