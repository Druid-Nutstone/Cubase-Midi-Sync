using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.WebSocket
{
    public static class WebSocketExtensions
    {
        public static string ForWebSocketSerialise(this string data)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
        public static string FromWebSocketDeserialise(this string data)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(data));
        }
    }
}
