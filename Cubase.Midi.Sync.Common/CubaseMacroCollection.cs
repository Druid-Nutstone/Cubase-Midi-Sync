using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common
{
    public class CubaseMacroCollection : List<CubaseMacro>
    {
        public CubaseMacro AddMacro(string name)
        {
            var latestNote = this.Select(x => x.Note).DefaultIfEmpty(0).Max();
            latestNote++;
            var newMacro = CubaseMacro.Create(name, latestNote);
            this.Add(newMacro);
            return newMacro;
        }

        public CubaseMacro GetMacroByName(string name)
        {
            return this.FirstOrDefault(x => x.Name == name);
        }

        public void SaveToFile(string fileName)
        {
            var asText = JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(fileName, asText);
        }

        public static CubaseMacroCollection LoadFromFile(string fileName)
        {
            return JsonSerializer.Deserialize<CubaseMacroCollection>(File.ReadAllText(fileName));
        }
    }


    public class CubaseMacro
    {
        public int Channel { get; set; } = 11;

        public int Note { get; set; }

        public string Name { get; set; }   
        
        public static CubaseMacro Create(string name, int note)
        {
            return new CubaseMacro() { Name = name, Note = note };
        }
    }
}
