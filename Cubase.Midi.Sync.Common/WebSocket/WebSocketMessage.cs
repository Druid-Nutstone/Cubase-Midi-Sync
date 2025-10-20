using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.WebSocket
{
    public class WebSocketMessage
    {
        public WebSocketCommand Command { get; set; }   
    
        public string? Data { get; set; }

        public string Message { get; set; }

        public static WebSocketMessage Create(WebSocketCommand command, object? data = null)
        {
            return new WebSocketMessage()
            {
                Command = command,
                Data = (string?)(data ?? JsonSerializer.Serialize(data))
            };
        }

        public static WebSocketMessage Deserialise(string message)
        {
            return JsonSerializer.Deserialize<WebSocketMessage>(message) 
                ?? throw new InvalidOperationException("Deserialization resulted in null.");
        }

        public string Serialise()
        {
            return JsonSerializer.Serialize(this);
        }

        public T GetMessage<T>()
        {
            if (string.IsNullOrEmpty(this.Data))
            {
                throw new InvalidOperationException("No data to deserialize.");
            }
            return JsonSerializer.Deserialize<T>(this.Data) ?? throw new InvalidOperationException("Deserialization resulted in null.");
        }

        public static WebSocketMessage CreateError(string message)
        {
            return new WebSocketMessage()
            {
                Message = message,
                Command = WebSocketCommand.Error    
            };
        }

    }


    public enum  WebSocketCommand 
    {
        Connected = 0,
        Error = 1,
        Commands = 2
    }
}
