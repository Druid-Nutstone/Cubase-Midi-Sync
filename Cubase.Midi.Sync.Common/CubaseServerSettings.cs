using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common
{
    public class CubaseServerSettings
    {
        public string FilePath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Cubase-Midi-Settings", "CubaseCommands.json");

        public string CubseServerLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Cubase-Midi-Settings", "CubaseMidiSyncServerLog.txt");

        public static string ScriptPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Cubase-Midi-Settings", "Scripts");

        public static string ScriptExtension = ".cql";
        
        public static int MaxNumberOfChannels = 48;    

        public static string KeyCommandsFileLocation { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Steinberg", "Cubase 15_64", "Key Commands.xml");

        public CubaseCommandsCollection GetVisibleCubaseCommands()
        {
            return CubaseCommandsCollection.CreateFromList(this.GetCubaseCommands().Where(c => c.Visible).ToList());
        }
        
        
       
        public CubaseCommandsCollection GetCubaseCommands()
        {
            var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Cubase-Midi-Settings");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);    
            }
            this.FilePath = Path.Combine(root, "CubaseCommands.json");
            if (File.Exists(this.FilePath))
            {
                return CubaseCommandsCollection.LoadFromFile(this.FilePath);
            }
            else
            {
                var emotyCollection = new CubaseCommandsCollection(); 
                emotyCollection.SaveToFile(this.FilePath);   
                return emotyCollection;
            }
        }
    }
}
