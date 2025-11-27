using System;
using System.Text;
using System.Text.Json;

namespace Cubase.Midi.Sync.Common.WebSocket
{
    public class WebSocketMessage
    {
        public WebSocketCommand Command { get; set; }

        // ✅ Use byte[] instead of string for large payloads
        public byte[]? Data { get; set; }

        public string? Message { get; set; }

        /// <summary>
        /// Create a new WebSocketMessage with optional data object.
        /// The object is serialized to JSON and stored as UTF8 bytes.
        /// </summary>
        public static WebSocketMessage Create(WebSocketCommand command, object? data = null)
        {
            return new WebSocketMessage
            {
                Command = command,
                Data = data != null
                    ? Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data))
                    : null
            };
        }

        /// <summary>
        /// Create a WebSocketMessage representing an error.
        /// </summary>
        public static WebSocketMessage CreateError(string errorMessage)
        {
            return new WebSocketMessage
            {
                Command = WebSocketCommand.Error,
                Message = errorMessage
            };
        }

        /// <summary>
        /// Deserialize a JSON string into a WebSocketMessage.
        /// </summary>
        public static WebSocketMessage Deserialise(string message)
        {
            try
            {
                return JsonSerializer.Deserialize<WebSocketMessage>(message)!;
            }
            catch (Exception ex)
            {
                return CreateError("Deserialisation error: " + ex.Message);
            }
        }

        /// <summary>
        /// Serialize this message to JSON string.
        /// </summary>
        public string Serialise()
            => JsonSerializer.Serialize(this);

        /// <summary>
        /// Convert the Data bytes back into the original object.
        /// </summary>
        public T GetMessage<T>()
        {
            if (Data != null && Data.Length > 0)
            {
                var json = Encoding.UTF8.GetString(Data);
                return JsonSerializer.Deserialize<T>(json)!;
            }

            return default!;
        }
    }

    public enum WebSocketCommand
    {
        Connected = 0,
        Error = 1,
        Commands = 2,
        Success = 3,
        ExecuteCubaseAction = 4,
        Tracks = 5,
        Mixer = 6,
        Windows = 7,
        ServerClosed = 8,
        CubaseNotReady = 9,
        CubaseReady = 10,
        TracksComplete = 11,
        SelectTracks = 12,
        TrackState = 13
    }
}
