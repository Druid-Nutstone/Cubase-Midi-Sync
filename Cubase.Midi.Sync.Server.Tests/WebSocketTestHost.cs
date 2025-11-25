using Cubase.Midi.Sync.Server.Services.Cache;
using Cubase.Midi.Sync.Server.Services.Midi;
using Cubase.Midi.Sync.Server.Services.WebSockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketTestHost : IAsyncDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    public ClientWebSocket WebSocket { get; private set; }
    public Uri WebSocketUri { get; private set; }

    public WebSocketTestHost(int port = 8014, string path = "/ws/midi")
    {

        WebSocketUri = new Uri($"ws://localhost:{port}{path}");
    }

    public async Task StartAsync()
    {
        var app = Program.BuildHost(); // returns IHost
        await app.StartAsync();

        await Task.Delay(2000);
        // Connect WebSocket
        WebSocket = new ClientWebSocket();
        await WebSocket.ConnectAsync(WebSocketUri, CancellationToken.None);
    }

    public async ValueTask DisposeAsync()
    {
        if (WebSocket != null)
        {
            if (WebSocket.State == WebSocketState.Open)
                await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test finished", CancellationToken.None);

            WebSocket.Dispose();
        }

        _factory.Dispose();
    }

    public async Task SendAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task<string> ReceiveAsync(int bufferSize = 4096)
    {
        var buffer = new byte[bufferSize];
        var result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }
}

