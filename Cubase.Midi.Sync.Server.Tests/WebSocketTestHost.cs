using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Common.Window;
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

    public async Task StartAsync(Func<WebSocketMessage, Task> msgHandler)
    {
        var app = Program.BuildHost(); // returns IHost
        await app.StartAsync();

        await Task.Delay(2000);
        // Connect WebSocket
        WebSocket = new ClientWebSocket();
        await WebSocket.ConnectAsync(WebSocketUri, CancellationToken.None);
        await WaitForResponse(msgHandler);
    }

    public async Task WaitForResponse(Func<WebSocketMessage, Task> msgHandler)
    {
        Task.Run(async () =>
        {
            var buffer = new byte[8192];

            while (WebSocket.State == WebSocketState.Open)
            {
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    using var reader = new StreamReader(ms, Encoding.UTF8);
                    string message = await reader.ReadToEndAsync();

                    var wsMessage = WebSocketMessage.Deserialise(message);
                    await msgHandler(wsMessage);
                }
            }
        });
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

