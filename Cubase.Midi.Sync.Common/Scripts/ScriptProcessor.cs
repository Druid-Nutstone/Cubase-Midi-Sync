using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Scripts
{
    public class ScriptProcessor : IScriptProcessor
    {
       
        public ScriptNode? GetScript(IEnumerable<string> script, Action<ScriptParseException> errorHandler)
        {
            var parser = new CommandParser();
            return parser.Parse(script, errorHandler);
        }
    }
}
