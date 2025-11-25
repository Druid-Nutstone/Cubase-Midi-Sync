using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Scripts
{
    public abstract class Node { }

    public class ScriptNode
    {
        public List<Node> Statements { get; set; } = new();
    }

    public class IfNode : Node
    {
        public Node Condition { get; set; }
        public List<Node> Then { get; set; } = new();
        public List<Node> Else { get; set; } = new();
    }

    public class CommandNode : Node
    {
        public string Command { get; set; }
        public List<string> Args { get; set; } = new();
    }
}
