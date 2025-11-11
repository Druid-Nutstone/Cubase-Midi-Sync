using Cubase.Midi.Sync.Common.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Mixer
{
    public class CubaseMixerResponse
    {
        public string Error { get; set; }   

        public CubaseMixerCommand Command { get; set; }

        public string DataAsString { get; set; }

        public T GetData<T>()
        {
            if (string.IsNullOrEmpty(DataAsString))
            {
                return default(T);
            }
            return JsonSerializer.Deserialize<T>(DataAsString.FromWebSocketDeserialise());
        }

        public static CubaseMixerResponse Create(CubaseMixerCommand command)
        {
            return new CubaseMixerResponse
            {
                Command = command,
            };
        }

        public static CubaseMixerResponse Create(CubaseMixerCommand command, object data)
        {
            return new CubaseMixerResponse
            {
                Command = command,
                DataAsString = JsonSerializer.Serialize(data).ForWebSocketSerialise()
            };
        }

        public static CubaseMixerResponse CreateError(string error)
        {
            return new CubaseMixerResponse
            {
               Error = error
            };
        }
    }
}
