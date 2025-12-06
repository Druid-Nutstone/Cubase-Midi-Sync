using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
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

        public MidiChannelCollection? Tracks { get; set; } = null;

        private Func<string, Task>? errorHandler = null;

        private readonly List<Func<MidiChannel, Task>> registeredTrackHandlers = new();

        private readonly List<Func<CubaseActiveWindowCollection, Task>> registeredWindowEventHandlers
            = new();

        private readonly List<Func<WebSocketCommand, Task>> registeredSystemMessageHandlers
            = new();

        public async Task ProcessWebSocket(WebSocketMessage request)
        {
            switch (request.Command)
            {
                case WebSocketCommand.TrackUpdated:
                    var midiChannel = request.GetMessage<MidiChannel>();
                    foreach (var trackHandler in this.registeredTrackHandlers)
                        _ = Task.Run(() => trackHandler(midiChannel));  // fire & forget safely
                    break;
                case WebSocketCommand.Tracks:
                    this.Tracks = request.GetMessage<MidiChannelCollection>();
                    break;
                case WebSocketCommand.ServerClosed:
                case WebSocketCommand.CubaseReady:
                case WebSocketCommand.CubaseNotReady: 
                    foreach (var systemMessageHandler in this.registeredSystemMessageHandlers)
                    {
                        await systemMessageHandler(request.Command);
                    }
                    break;
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
                        await windowHandler(cubaseWindowCollection);
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

        public void RegisterForSystemMessages(Func<WebSocketCommand, Task> systemMessageHandler)
        {
            if (!this.registeredSystemMessageHandlers.Contains(systemMessageHandler))
            {
                this.registeredSystemMessageHandlers.Add(systemMessageHandler);
            }
        }

        public void RegisterCubaseWindowHandler(Func<CubaseActiveWindowCollection, Task> windowHander)
        {
            if (!this.registeredWindowEventHandlers.Contains(windowHander))
            {
                this.registeredWindowEventHandlers.Add(windowHander);   
            }  
        }

        public void RegisterdTrackHandler(Func<MidiChannel, Task> trackHandler)
        {
            if (!this.registeredTrackHandlers.Contains(trackHandler))
            {
                this.registeredTrackHandlers.Add(trackHandler);
            }
        }

    }
}
