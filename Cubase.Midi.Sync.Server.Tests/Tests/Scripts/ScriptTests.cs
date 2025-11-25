using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Scripts;
using Cubase.Midi.Sync.Common.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Server.Tests.Tests.Scripts
{
    [TestClass]
    public class ScriptTests : BaseWebSocket
    {
        char quote = '"';


        [TestMethod]
        public async Task Can_Parse_Scripts()
        {
            await StartServer();
            
            var response = await SendScript("TestScript");

            /*
            var scripter = new ScriptProcessor();
            var testScript = new List<string>() {
                 $"If Select {quote}Telecaster{quote} = true then",
                 $"Open {quote}Telecaster{quote}",
                 "else Exit",
                 "endif"
            };
            scripter.GetScript(testScript, (error) => { });
            */
        }
    }
}
