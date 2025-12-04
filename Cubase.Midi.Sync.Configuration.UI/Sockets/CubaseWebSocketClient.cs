using Cubase.Midi.Sync.Common.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cubase.Midi.Sync.Configuration.UI.Sockets
{
    public class CubaseWebSocketClient
    {
        private static CubaseWebSocketClient client;

        private ClientWebSocket _ws;

        private CubaseWebSocketResponse responseHandler;

        public CubaseWebSocketClient()
        {
            this.responseHandler = new CubaseWebSocketResponse();
            _ws = new ClientWebSocket();
            Connect(this.responseHandler.ProcessResponse);
        }

        public static CubaseWebSocketClient Instance 
        {
            get
            {
                if (client == null)
                {
                    client = new CubaseWebSocketClient();

                }
                return client;
            } 
        }

        private CubaseWebSocketClient Connect(Action<WebSocketMessage> msgHandler)
        {
            var url = $"ws://127.0.0.1:8014/ws/midi";
            _ws.ConnectAsync(new Uri(url), CancellationToken.None)
               .Wait();
            RecieveLoop(msgHandler);
            return this;
        }

        public async Task<WebSocketMessage> SendCommand(WebSocketMessage message)
        {
            var commandString = message.Serialise();

            if (_ws.State == WebSocketState.Open)
            {
                try
                {
                    var data = Encoding.UTF8.GetBytes(commandString);
                    await _ws.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                    return WebSocketMessage.Create(WebSocketCommand.Success);
                }
                catch (Exception ex)
                {
                    return WebSocketMessage.CreateError("Send failed: " + ex.Message);
                }
            }
            else
            {
                return WebSocketMessage.CreateError("WebSocket not connected!");
            }
        } 

        public void RecieveLoop(Action<WebSocketMessage> action)
        {
            Task.Run(async () =>
            {
                var buffer = new byte[8192];
                while (_ws.State == WebSocketState.Open)
                {
                    using var ms = new MemoryStream();
                    WebSocketReceiveResult result;

                    do
                    {
                        result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        ms.Write(buffer, 0, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        using var reader = new StreamReader(ms, Encoding.UTF8);
                        string message = await reader.ReadToEndAsync();

                        var wsMessage = WebSocketMessage.Deserialise(message);
                        if (action != null)
                        {
                            action.Invoke(wsMessage);
                        }
                    }
                }
            });
        }
    }
}
