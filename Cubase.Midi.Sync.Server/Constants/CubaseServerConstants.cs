using System.Reflection;

namespace Cubase.Midi.Sync.Server.Constants
{
    public static class CubaseServerConstants
    {
        public static string CommandsFileLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CubaseCommands.json");

        public static string CubaseExeName = "Cubase14";

        public static string KeyMappingFileLocation= Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CubaseKeyMap.json");

        public static string KeyCommandsFileLocation { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steinberg", "Cubase 14_64", "Key Commands.xml");


    }
}
