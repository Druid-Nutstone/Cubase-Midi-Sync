using Cubase.Midi.Sync.Common.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Mixer
{
    public class CubaseMixerRequest
    {
        public CubaseMixerCommand Command { get; set; }

        public string TargetMixer { get; set; } 

        public string DataAsString { get; set; }

        public T GetData<T>()
        {
            if (string.IsNullOrEmpty(DataAsString))
            {
                return default(T);
            }
            return JsonSerializer.Deserialize<T>(DataAsString.FromWebSocketDeserialise());
        }

        public static CubaseMixerRequest Create(CubaseMixerCommand command)
        {
            return new CubaseMixerRequest
            {
                Command = command,
            };
        }

        public static CubaseMixerRequest Create(CubaseMixerCommand command, object data)
        {
            return new CubaseMixerRequest
            {
                Command = command,
                DataAsString = JsonSerializer.Serialize(data).ForWebSocketSerialise()
            };
        }

        public static CubaseMixerRequest Create(CubaseMixerCommand command, object data, string targetMixer)
        {
            return new CubaseMixerRequest
            {
                Command = command,
                DataAsString = JsonSerializer.Serialize(data).ForWebSocketSerialise(),
                TargetMixer = targetMixer
            };
        }
    }

    public enum CubaseMixerCommand
    {
        MixerCollection = 0,
        OpenMixer = 1,
        MixerStaticCommand = 2,
        FocusMixer = 3,
        Error = 99,
    }
}
