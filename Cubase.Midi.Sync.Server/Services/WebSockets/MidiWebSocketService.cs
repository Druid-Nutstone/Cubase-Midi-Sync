using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Server.Services.Cubase;
using Cubase.Midi.Sync.Server.Services.Windows;
using global::Cubase.Midi.Sync.Server.Services.Midi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Text;

namespace Cubase.Midi.Sync.Server.Services.WebSockets
{
    // Hosted service remains a simple background service (no DI resolution of WebApplication).
    // It can be used to manage lifecycle or background tasks; middleware is configured by WebSocketServer.Configure().
    public class MidiWebSocketService : BackgroundService
    {
        private readonly ILogger<MidiWebSocketService> _logger;

        public MidiWebSocketService(ILogger<MidiWebSocketService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MidiWebSocketService started.");
            try
            {
                // Keep the hosted service alive; do not try to modify the web pipeline here.
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) { /* expected on shutdown */ }
            _logger.LogInformation("MidiWebSocketService stopping.");
        }
    }

    // WebSocketServer no longer tries to resolve WebApplication from IServiceProvider.
    // It exposes a Configure method that Program.cs calls with the real `app`.
    public class WebSocketServer : IWebSocketServer
    {
        private readonly ILogger<WebSocketServer> _logger;
        private readonly IServiceProvider _services;
        private readonly ICubaseService cubaseService;
        private readonly IMidiService midiService;
        private readonly ICubaseWindowMonitor cubaseWindowMonitor;

        private List<WebSocket> _sockets = new List<WebSocket>();

        public WebSocketServer(ILogger<WebSocketServer> logger, 
                               IServiceProvider services, 
                               ICubaseService cubaseService,
                               ICubaseWindowMonitor cubaseWindowMonitor, 
                               IMidiService midiService)
        {
            _logger = logger;
            _services = services;
            this.midiService = midiService;
            this.cubaseService = cubaseService;
            this.cubaseWindowMonitor = cubaseWindowMonitor; 
        }

        public void BroadcastMessage(WebSocketMessage message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message.Serialise());
            var messageSegment = new ArraySegment<byte>(messageBytes);
            foreach (var ws in _sockets.ToList())
            {
                if (ws.State == WebSocketState.Open)
                {
                    ws.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None).Wait();
                }
                else
                {
                    _sockets.Remove(ws);
                }
            }
        }

        // Call this from Program.cs: wsServer.Configure(app);
        public void Configure(IApplicationBuilder app)
        {
            app.UseWebSockets();

            // Map the endpoint and use the HttpContext.RequestServices for per-request scope.
            app.Map("/ws/midi", builder =>
            {
                
                builder.Run(async context =>
                {
                    WebSocket ws = null;
                    try
                    {
                        if (!context.WebSockets.IsWebSocketRequest)
                        {
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            return;
                        }

                        ws = await context.WebSockets.AcceptWebSocketAsync();
                        this._sockets.Add(ws);
                        _logger.LogInformation("WebSocket connected from {ip}", context.Connection.RemoteIpAddress);

                        var buffer = new byte[1024 * 10];
                        var ct = context.RequestAborted;

                        var requestServices = context.RequestServices;

                        this.midiService.OnChannelChanged = async (channels) =>
                        {
                            if (ws.State == WebSocketState.Open)
                            {
                                var message = WebSocketMessage.Create(WebSocketCommand.Tracks, channels);
                                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message.Serialise())), WebSocketMessageType.Text, true, ct);
                            }
                        };

                        this.cubaseWindowMonitor.RegisterForWindowEvents(async (windows) =>
                        {
                            if (ws.State == WebSocketState.Open)
                            {
                                var message = WebSocketMessage.Create(WebSocketCommand.Windows, windows);
                                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message.Serialise())), WebSocketMessageType.Text, true, ct);
                            }
                        });

                        while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
                        {
                            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", ct);
                                _logger.LogInformation("WebSocket closed by client.");
                            }
                            else
                            {
                                var messageAsString = Encoding.UTF8.GetString(buffer, 0, result.Count);

                                var sourceMessage = WebSocketMessage.Deserialise(messageAsString);

                                _logger.LogInformation($"Received WS MIDI command: {sourceMessage.Command}");

                                var responseMessage = await cubaseService.ExecuteWebSocketAsync(sourceMessage);

                                // var response = Encoding.UTF8.GetBytes("ACK: " + message);
                                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(responseMessage.Serialise())), WebSocketMessageType.Text, true, ct);
                            }
                        }
                        if (ct.IsCancellationRequested)
                        {
                            _logger.LogInformation("WebSocket connection cancelled.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "WebSocket error: {message}", ex.Message);
                    }
                    finally
                    {
                        _logger.LogInformation("WebSocket connection closed.");
                        if (ws != null && ws.State != WebSocketState.Closed)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.InternalServerError, "Exception", CancellationToken.None);
                        }
                    }
                });
            });
        }
    }
}
