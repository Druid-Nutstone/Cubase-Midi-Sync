using Cubase.Midi.Sync.Common.WebSocket;

namespace Cubase.Midi.Sync.Server.Services.WebSockets
{
    public interface IWebSocketServer
    {
        void Configure(IApplicationBuilder app);

        void BroadcastMessage(WebSocketMessage message);
    }
}
