using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.WindowManager.Models
{
    public class WindowSnapCollection : List<WindowSnap>
    {
        public string Name { get; set; } 
    
        public WindowSnapCollection() { }
    


        public WindowSnapCollection CreateSnapCollection(List<string> windowNames)
        {
            var snapCollection = new WindowSnapCollection();    

            return snapCollection;
        }

        public static WindowSnapCollection Create(string name)
        {
            return new WindowSnapCollection()
            {
                Name = name
            };
        }
    }
}
