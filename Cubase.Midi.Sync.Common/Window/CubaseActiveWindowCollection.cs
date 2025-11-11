using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Window
{
    public class CubaseActiveWindowCollection : List<CubaseActiveWindow> 
    {
        public CubaseActiveWindow AddCubaseWindow(string name, CubaseWindowState state, CubaseWindowType type)
        {
            var cubaseWindow = new CubaseActiveWindow()
            {
                Name = name,
                State = state,
                Type = type
            };
            this.Add(cubaseWindow);
            return cubaseWindow;
        }

        public bool HaveAnyMixers()
        {
            return this.Any(x => x.Name.StartsWith("MixConsole", StringComparison.OrdinalIgnoreCase));
        }

        public int CountOfMixers()
        {
            return this.Count(x => x.Name.StartsWith("MixConsole", StringComparison.OrdinalIgnoreCase));
        }
    }

    public class CubaseActiveWindow
    {
        public string Name { get; set; }    

        public CubaseWindowState State { get; set; }    

        public CubaseWindowType Type { get; set; }
    }

    public enum CubaseWindowState
    {
        Hide = 0,
        Normal = 1,
        Minimized = 2,
        Maximized = 3,
        Restore = 9,
        Unknown = 99
    } 
    
    public enum CubaseWindowType
    {
        Primary = 0,
        Secondary = 1,
        Transiant = 2,
    }
}
