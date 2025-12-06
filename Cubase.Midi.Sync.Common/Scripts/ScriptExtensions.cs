using Cubase.Midi.Sync.Common.Midi;
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

        public static List<MidiChannel> ToMidiChannelArray(this object[] objects)
        {
            var list = new List<MidiChannel>();
            foreach ( object obj in objects )
            {
                if ( obj is MidiChannel )
                {
                    list.Add( (MidiChannel)obj );
                }
            }
            return list;
        }

        public static bool IsStringArray(this object[] args)
        {
            return args.Select(x => x.ToString()).Any();    
        }

        public static IEnumerable<string> ToStringArray(this object[] args)
        {
            var stringList = new List<string>();
            foreach (var arg in args)
            {
                if (arg is String)
                {
                    stringList.Add(arg.ToString()); 
                }
            }
            return stringList;
        }
    }
}
