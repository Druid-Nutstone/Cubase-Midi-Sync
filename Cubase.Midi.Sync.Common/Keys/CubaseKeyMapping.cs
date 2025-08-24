using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Keys
{
    public class CubaseMappingCollection : List<CubaseKeyMapping>
    {
        public CubaseMappingCollection AddMapping(VirtualKey key, CubaseActionName action)
        {
            this.Add(new CubaseKeyMapping() { CubaseKey = key, Command = action });
            return this;
        }
    
        public void SaveToFile(string fileName)
        {
            var asText = System.Text.Json.JsonSerializer.Serialize(this, new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
            System.IO.File.WriteAllText(fileName, asText);
        }

        public static CubaseMappingCollection LoadFromFile(string fileName)
        {
            return System.Text.Json.JsonSerializer.Deserialize<CubaseMappingCollection>(System.IO.File.ReadAllText(fileName)) ?? new CubaseMappingCollection();
        }

        public bool ContainsCubaseKey(CubaseActionName action)
        {
            return this.Any(x => x.Command == action);
        }

        public VirtualKey GetCubaseKey(CubaseActionName action)
        {
            var mapping = this.FirstOrDefault(x => x.Command == action);
            if (mapping == null) throw new Exception($"No mapping found for action {action}");
            return mapping.CubaseKey;
        }

        public static CubaseMappingCollection Create()
        {
            return new CubaseMappingCollection();
        }
    }

    public class CubaseKeyMapping
    {
        public VirtualKey CubaseKey { get; set; }   

        public CubaseActionName Command { get; set; }
    
        public string CubaseKeyString => CubaseKey.ToString();

        public string CommandString => Command.ToString();

    }
}
