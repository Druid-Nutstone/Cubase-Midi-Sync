using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Mixer;
using Cubase.Midi.Sync.Common.WebSocket;
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

        void RegisterForErrors(Func<string, Task> errorHandler);


    }
}
