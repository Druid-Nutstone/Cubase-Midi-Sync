using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.CubaseService.WebSocket
{
    public class MidiWebSocketResponse : IMidiWebSocketResponse
    {
        private CubaseCommandsCollection? commands = null;

        public async Task ProcessWebSocket(WebSocketMessage request)
        {
            await Task.Run(() => 
            {
                switch (request.Command)
                {
                    case WebSocketCommand.Commands:
                        this.commands = request.GetMessage<CubaseCommandsCollection>();
                        break;
                }
            }); 

        }

        public async Task<CubaseCommandsCollection> GetCommands()
        {
            while (this.commands == null)
            {
                await Task.Delay(5);
            }
            return this.commands;
        }
    }
}
