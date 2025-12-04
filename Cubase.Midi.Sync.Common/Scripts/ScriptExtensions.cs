using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Scripts
{
    public static class ScriptExtensions
    {
        public static IEnumerable<string> RemoveBlankLines(this IEnumerable<string> lines)
        {
            return lines.Where(line => line.Trim().Length > 0);    
        }

        public static ScriptFunction ToScriptFunction(this string command)
        {
            ScriptFunction func = ScriptFunction.Unknown;
            if (!Enum.TryParse<ScriptFunction>(command, true, out func))
            {
                return ScriptFunction.Unknown;
            }
            return func;
        }
    }
}
