using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Scripts
{
    public interface IScriptProcessor
    {

        ScriptNode? GetScript(IEnumerable<string> script, Action<ScriptParseException> errorHandler);


    }
}
