using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.UI.Settings
{
    public class AppSettings
    {
        public List<CubaseConnection> CubaseConnection { get; set; } = new List<CubaseConnection>();

        public string ActiveConnection { get; set; }

        public bool Connect { get; set;  } = true;  

    }


    public class CubaseConnection
    {
        public string Name { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }

        public string BaseUrl => $"http://{Host}:{Port}";
    }
}
