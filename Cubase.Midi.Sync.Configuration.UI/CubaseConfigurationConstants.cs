using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI
{
    public static class CubaseConfigurationConstants
    {
        public static string KeyCommandsFileLocation { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steinberg", "Cubase 15_64", "Key Commands.xml");

    }
}
