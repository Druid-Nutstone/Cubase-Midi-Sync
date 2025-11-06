using System.Reflection;

namespace Cubase.Midi.Sync.Server.Constants
{
    public static class CubaseServerConstants
    {
        public static string LogFileLocation { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Cubase-Midi-Settings");

        public static string CubaseExeName = "Cubase15";

        public static string KeyMappingFileLocation= Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CubaseKeyMap.json");

        public static string KeyCommandsFileLocation { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steinberg", "Cubase 15_64", "Key Commands.xml");


    }
}
