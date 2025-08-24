using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.Settings
{
    public class AppSettings
    {
        public CubaseConnection CubaseConnection { get; set; } = new CubaseConnection();
    }


    public class CubaseConnection
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public string BaseUrl => $"http://{Host}:{Port}";
    }
}
