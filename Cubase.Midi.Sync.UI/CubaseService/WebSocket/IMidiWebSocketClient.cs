using Cubase.Midi.Sync.Common.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.CubaseService.WebSocket
{
    public interface IMidiWebSocketClient
    {
        Task<WebSocketMessage> ConnectAsync();

        Task<WebSocketMessage> SendMidiCommand(WebSocketMessage message);

        Task<WebSocketMessage> ConnectIfNotConnectedAsync();


        Task Close(); 
    }
}
