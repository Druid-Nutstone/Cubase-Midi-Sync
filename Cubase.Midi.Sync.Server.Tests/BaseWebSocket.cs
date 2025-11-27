using Cubase.Midi.Sync.Common;
using Cubase.Midi.Sync.Common.Requests;
using Cubase.Midi.Sync.Common.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Base class for WebSocket tests in .NET 9
/// </summary>
public class BaseWebSocket 
{
    private WebSocketTestHost _host;

    public BaseWebSocket()
    {

    }

    public async Task StartServer(Func<WebSocketMessage, Task> msgHandler)
    {
        _host = new WebSocketTestHost();
        await _host.StartAsync(msgHandler);
    }

    public async Task<WebSocketMessage> SendSocketCommand(WebSocketMessage socketMessage)
    {
        return await Send(socketMessage);
    }

    public async Task<WebSocketMessage> SendCommand(CubaseCommand command)
    {
        var cubaseSocketRequest = CubaseActionRequest.CreateFromCommand(command);
        var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction, cubaseSocketRequest);
        return await Send(socketMessage);
    }

    public async Task<WebSocketMessage> SendScript(string scriptName)
    {
        CubaseCommand command = CubaseCommand.Create().WithAction(ActionEvent.Create(CubaseAreaTypes.Script, scriptName));
        var cubaseSocketRequest = CubaseActionRequest.CreateFromCommand(command);
        var socketMessage = WebSocketMessage.Create(WebSocketCommand.ExecuteCubaseAction, cubaseSocketRequest);
        return await Send(socketMessage);
    }

    private async Task<WebSocketMessage> Send(WebSocketMessage message)
    {
        var commandString = message.Serialise();

        if (_host.WebSocket.State == WebSocketState.Open)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(commandString);
                await _host.WebSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
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

} 


