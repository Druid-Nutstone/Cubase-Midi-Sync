using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Common.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.CubaseService.WebSocket
{
    public interface IMidiWebSocketResponse
    {
        Task ProcessWebSocket(WebSocketMessage request);
        Task<CubaseCommandsCollection> GetCommands();

        Task<CubaseMixerCollection> GetMixer();

        CubaseMixerCollection? mixerCollection { get; set; }

        void RegisterForErrors(Func<string, Task> errorHandler);

        void RegisterCubaseWindowHandler(Action<CubaseActiveWindowCollection> windowHander);

    }
}
