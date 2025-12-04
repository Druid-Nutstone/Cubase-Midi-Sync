using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.Scripts;
using Cubase.Midi.Sync.Common.Tracks;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Server.Constants;
using Cubase.Midi.Sync.Server.Services.CommandCategproes.Script;
using Cubase.Midi.Sync.Server.Services.Midi;
using Cubase.Midi.Sync.WindowManager.Models;
using Cubase.Midi.Sync.WindowManager.Services.Win;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Server.Tests.Tests.Scripts
{
    [TestClass]
    public class ScriptTests : BaseWebSocket
    {
        char quote = '"';

        bool cubaseIsUpAndReady = false;

        MidiChannelCollection channelCollection = null;

        bool tracksAreComplete = false;

        [TestMethod]
        public async Task Can_Parse_Scripts()
        {
            await StartServer(this.ProcessWebSocketResponse);
            
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

        [TestMethod]
        public async Task Can_Get_Tracks()
        {
            this.tracksAreComplete = false;
            this.channelCollection = null;
            await StartServer(this.ProcessWebSocketResponse);
            // wait for cubase to be up !!!!
            await this.StartCubase();

            await SendSocketCommand(WebSocketMessage.Create(WebSocketCommand.Tracks));
            
            var channelCollection = await this.GetTracks();

            var teletrack = channelCollection.GetTracksThatContain("tele");

            if (teletrack.Count > 0)
            {
                await SendSocketCommand(WebSocketMessage.Create(WebSocketCommand.SelectTracks, teletrack));
                var newChannelCollection = await this.GetTracks();
            }
        }

        [TestMethod]
        public async Task Can_Start_Cubase()
        {
            await StartCubase();
        }

        [TestMethod]
        public async Task Can_Run_Script()
        {
            // Test Select Track
            await StartServer(this.ProcessWebSocketResponse);
            // wait for cubase to be up !!!!
            await this.StartCubase();

            await Task.Delay(20000);
            
            var scriptResult = await SendCommand(CubaseCommand.Create()
                               .WithButtonType(CubaseButtonType.Script)
                               .WithAction(ActionEvent.Create(CubaseAreaTypes.Script, "Test Select Track")));
            
            if (scriptResult == null)
            {

            }
            
            //var sut = new CubaseScriptService();
            //var result = await sut.ProcessActionAsync(ActionEvent.Create(CubaseAreaTypes.Script, "Test Select Track"));
        }

        [TestMethod]
        public async Task Can_Send_Midi_Command()
        {
            await StartServer(this.ProcessWebSocketResponse);
            var collection = new MidiAndKeysCollection();
            var midiCommand = collection.GetMidiCommand(KnownCubaseMidiCommands.Reload_Scripts);
            await SendCommand(CubaseCommand.Create()
                                           .WithButtonType(CubaseButtonType.Momentory)
                                           .WithAction(ActionEvent.CreateFromMidiAndKey(midiCommand)));
        }

        private async Task ProcessWebSocketResponse(WebSocketMessage message)
        {
            switch (message.Command)
            {
                case WebSocketCommand.CubaseNotReady:
                    cubaseIsUpAndReady = false;
                    break;
                case WebSocketCommand.CubaseReady:
                    cubaseIsUpAndReady = true;
                    break;
                case WebSocketCommand.Success:
                    break;
                case WebSocketCommand.TracksComplete:
                    this.tracksAreComplete = true;
                    this.channelCollection = message.GetMessage<MidiChannelCollection>();
                    break;
            }
        }

        private async Task<MidiChannelCollection> GetTracks()
        {
            while (!this.tracksAreComplete)
            {
                await Task.Delay(1000);
            }
            return this.channelCollection;
        }

        private async Task StartCubase()
        {
            cubaseIsUpAndReady = false;
            Process p = new Process();
            p.StartInfo.FileName = "C:\\Program Files\\Steinberg\\Cubase 15\\Cubase15.exe";
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.Arguments = "C:\\Users\\david\\OneDrive\\Documents\\Cubase\\Cubase 15\\Test15\\Test15.cpr";
            p.Start();
            while (!cubaseIsUpAndReady)
            {
                await Task.Delay(1000);
            }
        }
    }
}
