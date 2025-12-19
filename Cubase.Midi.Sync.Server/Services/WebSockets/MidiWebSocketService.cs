using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Midi;
using Cubase.Midi.Sync.Common.WebSocket;
using Cubase.Midi.Sync.Common.Window;
using Cubase.Midi.Sync.Server.Services.Cubase;
using Cubase.Midi.Sync.Server.Services.Windows;
using global::Cubase.Midi.Sync.Server.Services.Midi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;

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

        private readonly SemaphoreSlim _wsSendLock = new(1, 1);

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

        public async Task BroadcastMessageAsync(WebSocketMessage message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message.Serialise());
            var messageSegment = new ArraySegment<byte>(messageBytes);
            var sockets = _sockets.ToList();
            foreach (var ws in sockets)
            {
                if (ws.State == WebSocketState.Open)
                {
                    try
                    {
                        await ws.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error broadcasting to websocket");
                    }
                }
                else
                {
                    _sockets.Remove(ws);
                }
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseWebSockets();

            app.Map("/ws/midi", builder =>
            {
                builder.Run(async context =>
                {
                    WebSocket ws = null;
                    var ct = context.RequestAborted;

                    // Event handlers (real types from your project)
                    Action<MidiChannelCollection> channelHandler = null;
                    Action<MidiChannel> trackHandler = null;
                    Action<CubaseActiveWindowCollection> windowHandler = null;

                    try
                    {
                        if (!context.WebSockets.IsWebSocketRequest)
                        {
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            return;
                        }

                        ws = await context.WebSockets.AcceptWebSocketAsync();
                        _sockets.Add(ws);
                        _logger.LogInformation("WebSocket connected from {ip}", context.Connection.RemoteIpAddress);

                        var buffer = new byte[1024 * 10];

                        // ======= CHANNEL HANDLER =======
                        channelHandler = async (channels) =>
                        {
                            if (ws.State != WebSocketState.Open) return;

                            await _wsSendLock.WaitAsync();
                            try
                            {
                                var message = WebSocketMessage.Create(WebSocketCommand.Tracks, channels);
                                await ws.SendAsync(
                                    Encoding.UTF8.GetBytes(message.Serialise()),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);

                                if (midiService.MidiChannels.Count == CubaseServerSettings.MaxNumberOfChannels)
                                {
                                    var allTracks = WebSocketMessage.Create(WebSocketCommand.TracksComplete, channels);
                                    await ws.SendAsync(
                                        Encoding.UTF8.GetBytes(allTracks.Serialise()),
                                        WebSocketMessageType.Text,
                                        true,
                                        CancellationToken.None);
                                }
                            }
                            catch (WebSocketException) { /* client disconnected */ }
                            catch (Exception ex) { _logger.LogError(ex, "Error sending channel update"); }
                            finally { _wsSendLock.Release(); }
                        };
                        midiService.RegisterOnChannelChanged(channelHandler);

                        // ======= TRACK HANDLER =======
                        trackHandler = async (track) =>
                        {
                            if (ws.State != WebSocketState.Open) return;

                            await _wsSendLock.WaitAsync();
                            try
                            {
                                var message = WebSocketMessage.Create(WebSocketCommand.TrackUpdated, track);
                                await ws.SendAsync(
                                    Encoding.UTF8.GetBytes(message.Serialise()),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);
                            }
                            catch (WebSocketException) { /* client disconnected */ }
                            catch (Exception ex) { _logger.LogError(ex, "Error sending track update"); }
                            finally { _wsSendLock.Release(); }
                        };
                        midiService.RegisterOnTrackChanged(trackHandler);

                        // ======= WINDOW HANDLER =======
                        windowHandler = async (windows) =>
                        {
                            if (ws.State != WebSocketState.Open) return;

                            await _wsSendLock.WaitAsync();
                            try
                            {
                                var message = WebSocketMessage.Create(WebSocketCommand.Windows, windows);
                                await ws.SendAsync(
                                    Encoding.UTF8.GetBytes(message.Serialise()),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);
                            }
                            catch (WebSocketException) { /* client disconnected */ }
                            catch (Exception ex) { _logger.LogError(ex, "Error sending window update"); }
                            finally { _wsSendLock.Release(); }
                        };
                        cubaseWindowMonitor.RegisterForWindowEvents(windowHandler);

                        // ======= RECEIVE LOOP =======
                        while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
                        {
                            WebSocketReceiveResult result;
                            try
                            {
                                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                            }
                            catch (OperationCanceledException) { break; }

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", ct);
                                _logger.LogInformation("WebSocket closed by client.");
                                break;
                            }

                            var messageAsString = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            var sourceMessage = WebSocketMessage.Deserialise(messageAsString);

                            _logger.LogInformation($"Received WS MIDI command: {sourceMessage.Command}");

                            var responseMessage = await cubaseService.ExecuteWebSocketAsync(sourceMessage);

                            await _wsSendLock.WaitAsync();
                            try
                            {
                                if (ws.State == WebSocketState.Open)
                                {
                                    await ws.SendAsync(
                                        Encoding.UTF8.GetBytes(responseMessage.Serialise()),
                                        WebSocketMessageType.Text,
                                        true,
                                        CancellationToken.None);
                                }
                            }
                            finally { _wsSendLock.Release(); }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "WebSocket error: {message}", ex.Message);
                    }
                    finally
                    {
                        _logger.LogInformation("WebSocket connection closed.");

                        if (ws != null)
                        {
                            if (ws.State != WebSocketState.Closed)
                            {
                                try
                                {
                                    await ws.CloseAsync(WebSocketCloseStatus.InternalServerError, "Closing", CancellationToken.None);
                                }
                                catch (WebSocketException ex)
                                {
                                    _logger.LogWarning("Error closing websocket: {msg}", ex.Message);
                                }
                            }

                            _sockets.Remove(ws);

                            // Unregister handlers to avoid ghost sends
                            if (channelHandler != null) midiService.UnRegisterOnChannelChanged(channelHandler);
                            if (trackHandler != null) midiService.UnRegisterOnTrackSelected(trackHandler);
                            if (windowHandler != null) cubaseWindowMonitor.UnRegisterForWindowEvents(windowHandler);
                        }
                    }
                });
            });
        }



        // Call this from Program.cs: wsServer.Configure(app);
        /*
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

                        this.midiService.RegisterOnChannelChanged(async (channels) =>
                        {
                            if (ws.State != WebSocketState.Open)
                                return;

                            await _wsSendLock.WaitAsync();
                            try
                            {
                                var message = WebSocketMessage.Create(WebSocketCommand.Tracks, channels);
                                await ws.SendAsync(
                                    Encoding.UTF8.GetBytes(message.Serialise()),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None); // 👈 important

                                if (this.midiService.MidiChannels.Count == CubaseServerSettings.MaxNumberOfChannels)
                                {
                                    var allTracks = WebSocketMessage.Create(WebSocketCommand.TracksComplete, channels);
                                    await ws.SendAsync(
                                        Encoding.UTF8.GetBytes(allTracks.Serialise()),
                                        WebSocketMessageType.Text,
                                        true,
                                        CancellationToken.None);
                                }
                            }
                            catch (WebSocketException)
                            {
                                // client disconnected – normal
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error sending channel update");
                            }
                            finally
                            {
                                _wsSendLock.Release();
                            }
                        });


                        this.midiService.RegisterOnTrackChanged(async (track) =>
                        {
                            if (ws.State != WebSocketState.Open)
                                return;

                            await _wsSendLock.WaitAsync();
                            try
                            {
                                var message = WebSocketMessage.Create(WebSocketCommand.TrackUpdated, track);
                                await ws.SendAsync(
                                    Encoding.UTF8.GetBytes(message.Serialise()),
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None);
                            }
                            catch (WebSocketException)
                            {
                                // normal disconnect
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error sending track update");
                            }
                            finally
                            {
                                _wsSendLock.Release();
                            }
                        });


                        this.cubaseWindowMonitor.RegisterForWindowEvents(async (windows) =>
                        {
                            try
                            {
                                if (ws.State == WebSocketState.Open)
                                {
                                    var message = WebSocketMessage.Create(WebSocketCommand.Windows, windows);
                                    await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message.Serialise())), WebSocketMessageType.Text, true, ct);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error sending window update: {ex.Message}", ex.Message);
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
                            try
                            {
                                await ws.CloseAsync(WebSocketCloseStatus.InternalServerError, "Exception", CancellationToken.None);
                            }
                            catch (WebSocketException ex)
                            {
                                this._logger.LogError($"Error closing - might not be an error {ex.Message}");
                            }
                            finally
                            {
                              // todo unregister 
                            }
                        }
                    }
                });
            });
        
        } */
    }

}
