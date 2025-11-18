using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.CubaseService.WebSocket
{
    using Cubase.Midi.Sync.Common.WebSocket;
    using Cubase.Midi.Sync.UI.Settings;
    using Microsoft.Maui.Controls;
    using System.Net.WebSockets;
    using System.Text;

    public class MidiWebSocketClient : IMidiWebSocketClient, IDisposable
    {
        private ClientWebSocket _ws = new ClientWebSocket();

        private readonly AppSettings appSettings;  

        private readonly IMidiWebSocketResponse midiWebSocketResponse;

        public MidiWebSocketClient(AppSettings appSettings, IMidiWebSocketResponse midiWebSocketResponse)
        {
            this.appSettings = appSettings;  
            this.midiWebSocketResponse = midiWebSocketResponse; 
        }

        public async Task<WebSocketMessage> ConnectAsync()
        {
            try
            {
                var connection = appSettings.CubaseConnection.First(x => x.Name.Equals(appSettings.ActiveConnection, StringComparison.OrdinalIgnoreCase));
                var url = $"ws://{connection.Host}:{connection.Port}/ws/midi";
                await _ws.ConnectAsync(new Uri(url), CancellationToken.None);
                _ = ReceiveLoop(); // Start receiving in background
                return WebSocketMessage.Create(WebSocketCommand.Connected);
            }
            catch (Exception ex)
            {
                return WebSocketMessage.CreateError("Connection failed: " + ex.Message);    
            }
        }

        private async Task ReceiveLoop()
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
                    await this.midiWebSocketResponse.ProcessWebSocket(wsMessage);
                }
            }
        }

        public async Task<WebSocketMessage> SendMidiCommand(WebSocketMessage message)
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

        public async Task SendMidiCommand(string command)
        {
            if (_ws.State == WebSocketState.Open)
            {
                var data = Encoding.UTF8.GetBytes(command);
                await _ws.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                Console.WriteLine("WebSocket not connected!");
            }
        }

        public async Task Close()
        {
            if (_ws != null && _ws.State == WebSocketState.Open)
            {
                // Send the Close frame to the server
                await _ws.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,  // Reason for closing
                    "Client closing",                   // Optional description
                    CancellationToken.None
                );

                // Dispose the WebSocket object
                _ws.Dispose();
                _ws = null;
            }
        }

        public void Dispose()
        {
            this.Close();
        }
    }

}
