using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Keys
{
    [Obsolete]
    public class CubaseMappingCollection : List<CubaseKeyMapping>
    {
        public CubaseMappingCollection AddMapping(string key, string category)
        {
            this.Add(new CubaseKeyMapping() { CubaseKey = key, Category = category });
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

        public bool ContainsCubaseKey(string action)
        {
            return false; // this.Any(x => x.Command == action);
        }

        public string GetCubaseKey(string action)
        {
            //var mapping = this.FirstOrDefault(x => x.Command == action);
            //if (mapping == null) throw new Exception($"No mapping found for action {action}");
            //return mapping.CubaseKey;
            return null;
        }

        public static CubaseMappingCollection Create()
        {
            return new CubaseMappingCollection();
        }
    }

    public class CubaseKeyMapping
    {
        public string CubaseKey { get; set; }   

        public string Category { get; set; }

    }
}
