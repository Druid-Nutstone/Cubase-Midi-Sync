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

        private Func<string, Task>? errorHandler = null;

        public async Task ProcessWebSocket(WebSocketMessage request)
        {
            switch (request.Command)
            {
                case WebSocketCommand.Commands:
                    this.commands = request.GetMessage<CubaseCommandsCollection>();
                    break;
                case WebSocketCommand.Error:
                    var errorMessage = request.Message ?? "Unknown error";
                    await this.errorHandler?.Invoke(errorMessage);
                    break;
            }

        }

        public async Task<CubaseCommandsCollection> GetCommands()
        {
            while (this.commands == null)
            {
                await Task.Delay(50);
            }
            return this.commands;
        }

        public void RegisterForErrors(Func<string, Task> errorHandler)
        {
            this.errorHandler = errorHandler;
        }
    }
}
