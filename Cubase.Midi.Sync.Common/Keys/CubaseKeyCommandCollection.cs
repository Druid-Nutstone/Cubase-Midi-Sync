using Cubase.Midi.Sync.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Keys
{
    public class CubaseKeyCommandCollection : List<CubaseKeyCommand>    
    {
        public List<CubaseKeyCommand> GetAllocated()
        {
            return this.Where(c => !string.IsNullOrWhiteSpace(c.Key)).ToList(); 
        }

        public List<string> GetCategories()
        {
            return this.Select(c => c.Category).Distinct().OrderBy(c => c).ToList();
        }
        
        public List<string> GetKeys()
        {
            return this.Select(c => c.Key).Where(k => !string.IsNullOrWhiteSpace(k)).Distinct().OrderBy(k => k).ToList();
        }

        public CubaseKeyCommand GetByCategoryAndName(string category, string name)
        {
            return this.FirstOrDefault(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public List<CubaseKeyCommand> GetByCategory(string category)
        {
            return this.Where(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).OrderBy(c => c.Name).ToList();
        }

        public List<CubaseKeyCommand> GetByKey(string key)
        {
            return this.Where(x => x.Key.Contains(key, StringComparison.OrdinalIgnoreCase)).ToList();   
        }

        public List<CubaseKeyCommand> GetByCubaseDescription(string description)
        {
            return this
                     .Where(x => x.CubaseCommand?.CommandDescription?
                     .Contains(description, StringComparison.OrdinalIgnoreCase) == true)
                     .ToList();
        }

        public bool IsInCubase(CubaseKnownCommand command, string key)
        {
            var existingCommand = this.FirstOrDefault(x => x.CubaseCommand?.CommandName == command.CommandName);
            if (existingCommand != null) 
            {
                return existingCommand.Key.Equals(key);
            }
            return false;
        }

        public List<CubaseKeyCommand> GetByName(string name)
        {
            return this.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).OrderBy(c => c.Name).ToList();
        }

    }


    public class CubaseKeyCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Category {  get; set; } = string.Empty;
        public ActionEvent Action { get; set; }

        public CubaseKnownCommand? CubaseCommand { get; set; }

        public static CubaseKeyCommand Create()
        {
            return new CubaseKeyCommand();
        }

        public static CubaseKeyCommand CreateFromMidiAndKey(MidiAndKey midiAndKey)
        {
            return new CubaseKeyCommand()
            {
                Category = midiAndKey.Category,
                Name = midiAndKey.Name,
                Key = midiAndKey.Action,
                Action = ActionEvent.CreateFromMidiAndKey(midiAndKey)
            };
        }
    }
}
