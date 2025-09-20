using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Keys
{
    public class RequiredKeyMappingCollection : List<RequiredKey>
    {
        public RequiredKeyMappingCollection()
        {
            var commands = CubaseKeyCommandParser.Create().Parse();
            this.Add(new RequiredKey() { Id = RequiredKeyId.Mixer_Hide_All, Category = "Mixer", Name = "Hide: Hide All" });
            this.Add(new RequiredKey() { Id = RequiredKeyId.Mixer_Show_All, Category = "Mixer", Name = "Hide: Reveal All" });
            var hideAll = this.GetById(RequiredKeyId.Mixer_Hide_All);
            hideAll.WithKey(commands.GetByCategoryAndName(hideAll.Category, hideAll.Name).Key);
            var showAll = this.GetById(RequiredKeyId.Mixer_Show_All);
            showAll.WithKey(commands.GetByCategoryAndName(showAll.Category, showAll.Name).Key);
        }

        public bool AreAllKeysDefined()
        {
            return !this.Any(x => string.IsNullOrEmpty(x.Key));
        }

        public List<RequiredKey> GetUndefinedKeys()
        {
            return this.Where(x => string.IsNullOrEmpty(x.Key)).ToList();
        }

        public RequiredKey GetById(RequiredKeyId key)
        {
            return this.FirstOrDefault(x => x.Id == key);
        }

        public string GetKey(RequiredKeyId id)
        {
            return this.GetById(id).Key;
        }

        public static RequiredKeyMappingCollection Create()
        {
            return new RequiredKeyMappingCollection();
        }
    }

    public class RequiredKey
    {
        public RequiredKeyId Id { get; set; }  
        
        public string Category { get; set; }

        public string Name { get; set; }

        public string Key { get; set; } 

        public RequiredKey WithKey(string key)
        {
            this.Key = key;
            return this;
        }


    }

    public enum RequiredKeyId
    {
        Mixer_Hide_All,
        Mixer_Show_All
    }
}
