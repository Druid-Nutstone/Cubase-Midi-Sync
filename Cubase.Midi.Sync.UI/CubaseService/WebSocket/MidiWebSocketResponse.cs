using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Common.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.CubaseService.WebSocket
{
    public class MidiWebSocketResponse : IMidiWebSocketResponse
    {
        private CubaseCommandsCollection? commands = null;

        public CubaseMixerCollection? mixerCollection { get; set; } = null; 

        private Func<string, Task>? errorHandler = null;

        private List<Action<CubaseActiveWindowCollection>> registeredWindowEventHandlers = new List<Action<CubaseActiveWindowCollection>>();

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
                case WebSocketCommand.Mixer:
                    var mixerResponse = request.GetMessage<CubaseMixerResponse>();
                    switch (mixerResponse.Command)
                    {
                        case CubaseMixerCommand.MixerCollection:
                            this.mixerCollection = mixerResponse.GetData<CubaseMixerCollection>();
                            break;
                    }
                    break;
                case WebSocketCommand.Windows:
                    var cubaseWindowCollection = request.GetMessage<CubaseActiveWindowCollection>();
                    foreach (var windowHandler in this.registeredWindowEventHandlers)
                    {
                        windowHandler.Invoke(cubaseWindowCollection);
                    }
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

        public async Task<CubaseMixerCollection> GetMixer()
        {
            while (this.mixerCollection == null)
            {
                await Task.Delay(50);
            }
            return this.mixerCollection;
        }

        public void RegisterForErrors(Func<string, Task> errorHandler)
        {
            this.errorHandler = errorHandler;
        }

        public void RegisterCubaseWindowHandler(Action<CubaseActiveWindowCollection> windowHander)
        {
            if (!this.registeredWindowEventHandlers.Contains(windowHander))
            {
                this.registeredWindowEventHandlers.Add(windowHander);   
            }  
        }
    }
}
