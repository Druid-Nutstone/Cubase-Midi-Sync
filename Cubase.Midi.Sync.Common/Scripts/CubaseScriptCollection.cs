using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Scripts
{
    public class CubaseScriptCollection : List<CubaseScript>
    {
        public CubaseScriptCollection() 
        {
            var scriptFiles = Directory.GetFiles(CubaseServerSettings.ScriptPath, $"*{CubaseServerSettings.ScriptExtension}", new EnumerationOptions() { RecurseSubdirectories = true })
                     .Select(x => new CubaseScript() { FileName = x, Name = Path.GetFileNameWithoutExtension(x) });
            this.AddRange(scriptFiles);
        }  
    }

    public class CubaseScript()
    {
        public string FileName { get; set; }    
    
        public string Name { get; set; }
    }
}
