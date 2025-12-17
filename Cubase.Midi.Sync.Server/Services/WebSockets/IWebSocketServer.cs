using Cubase.Midi.Sync.Common.WebSocket;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Server.Services.WebSockets
{
    public interface IWebSocketServer
    {
        void Configure(IApplicationBuilder app);

        Task BroadcastMessageAsync(WebSocketMessage message);
    }
}
