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

    private CancellationTokenSource _receiveCts;
    private Task _receiveTask;

    public WebSocketTestHost(int port = 8014, string path = "/ws/midi")
    {

        WebSocketUri = new Uri($"ws://localhost:{port}{path}");
    }

    public async Task StartAsync(Func<WebSocketMessage, Task> msgHandler, CancellationToken cancellationToken = default)
    {
        var app = Program.BuildHost(); // returns IHost
        await app.StartAsync(cancellationToken);

        await Task.Delay(2000, cancellationToken);
        // Connect WebSocket
        WebSocket = new ClientWebSocket();
        await WebSocket.ConnectAsync(WebSocketUri, cancellationToken);

        // Start background receiver task with its own CTS so it can be stopped independently
        _receiveCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _receiveTask = ReceiveLoopAsync(msgHandler, _receiveCts.Token);
    }

    private async Task ReceiveLoopAsync(Func<WebSocketMessage, Task> msgHandler, CancellationToken cancellationToken)
    {
        try
        {
            var buffer = new byte[8192];

            while (WebSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage && !cancellationToken.IsCancellationRequested);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    using var reader = new StreamReader(ms, Encoding.UTF8);
                    string message = await reader.ReadToEndAsync();

                    var wsMessage = WebSocketMessage.Deserialise(message);
                    await msgHandler(wsMessage);
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // normal cancellation — ignore
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"WebSocket receive loop error: {ex.Message}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_receiveCts != null)
            {
                _receiveCts.Cancel();
                if (_receiveTask != null)
                    await _receiveTask.ConfigureAwait(false);
                _receiveCts.Dispose();
            }

            if (WebSocket != null)
            {
                if (WebSocket.State == WebSocketState.Open)
                    await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test finished", CancellationToken.None);

                WebSocket.Dispose();
            }

            _factory?.Dispose();
        }
        catch { }
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

