using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Window
{
    public class CubaseActiveWindowCollection : List<CubaseActiveWindow> 
    {
        public CubaseActiveWindow AddCubaseWindow(string name, CubaseWindowState state, CubaseWindowType type, CubaseWindowZOrder zOrder)
        {
            var cubaseWindow = new CubaseActiveWindow()
            {
                Name = name,
                State = state,
                Type = type,
                ZOrder = zOrder
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

        public bool Compare(CubaseActiveWindowCollection other)
        {
            this doesn;T work 
            var areEqual = this.ComputeHash().Equals(other.ComputeHash());
            return areEqual;
        }

        public string ComputeHash()
        {
            var json = JsonSerializer.Serialize(this);
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(json);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public List<CubaseActiveWindow> GetAllMixers()
        {
            return this.Where(x => x.Name.StartsWith("MixConsole", StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }

    public class CubaseActiveWindow
    {
        public string Name { get; set; }    

        public CubaseWindowState State { get; set; }    

        public CubaseWindowType Type { get; set; }

        public CubaseWindowZOrder ZOrder { get; set; }
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

    public enum CubaseWindowZOrder
    {
        Focused = 0,
        Active = 1,
        Unknown = 99
    }
}
