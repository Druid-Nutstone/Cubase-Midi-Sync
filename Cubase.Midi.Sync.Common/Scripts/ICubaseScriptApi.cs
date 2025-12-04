using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Scripts
{
    public interface ICubaseScriptApi
    {
        Task<ScriptResult> ExecuteCommandAsync(string command, params object[] args);
        Task<object> CallFunctionAsync(string function, params object[] args);
    }
}
