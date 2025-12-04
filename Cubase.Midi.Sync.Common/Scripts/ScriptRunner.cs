using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Common.Scripts
{
    public class ScriptRunner
    {
        private readonly ICubaseScriptApi _cubase; // Your WebSocket client interface
        private readonly Dictionary<string, object> _variables = new();

        public ScriptRunner(ICubaseScriptApi cubase)
        {
            _cubase = cubase;
        }

        public async Task<ScriptResult> ExecuteAsync(ScriptNode script)
        {
            foreach (var stmt in script.Statements)
            {
                var result = await ExecuteNodeAsync(stmt);
                if (!result.IsSucces)
                {
                    return result;
                }
            }
            return ScriptResult.Create();
        }

        private async Task<ScriptResult> ExecuteNodeAsync(Node node)
        {
            switch (node)
            {
                case LetNode letNode:
                    var value = await EvaluateExpressionAsync(letNode.Expression);
                    _variables[letNode.Variable] = value;
                    return ScriptResult.Create();
                case CommandNode cmdNode:
                    var args = new List<object>();
                    foreach (var arg in cmdNode.Args)
                    {
                        var evaluate = await EvaluateExpressionAsync(arg);
                        if (evaluate is ScriptResult)
                        {
                            return (ScriptResult)evaluate;
                        }
                        args.Add(evaluate);
                    }
                    return await _cubase.ExecuteCommandAsync(cmdNode.Command, args.ToArray());
                case IfNode ifNode:
                    if (EvaluateCondition(ifNode.Condition))
                        return await ExecuteBlockAsync(ifNode.Then);
                    else if (ifNode.Else != null)
                        return await ExecuteBlockAsync(ifNode.Else);
                    return ScriptResult.CreateError("Invalid if/else");

                case ForEachNode foreachNode:
                    var collection = await EvaluateExpressionAsync(foreachNode.Collection) as IEnumerable<object>;
                    if (collection == null)
                    {
                        return ScriptResult.CreateError("Cannot evaluate Foreach");
                    }

                    if (collection is ScriptResult)
                    {
                        return (ScriptResult)collection;
                    }

                    foreach (var item in collection)
                    {
                        _variables[foreachNode.Variable] = item;
                        var result = await ExecuteBlockAsync(foreachNode.Body);
                        if (!result.IsSucces)
                        {
                            return result;
                        }
                    }
                    _variables.Remove(foreachNode.Variable);
                    return ScriptResult.Create();

                default:
                    return ScriptResult.CreateError($"Unknown node type: {node.GetType()}");
            }
            // return ScriptResult.CreateError($"Could not evaluate Node {node.GetType()}");
        }

        private async Task<ScriptResult> ExecuteBlockAsync(List<Node> block)
        {
            foreach (var stmt in block)
            {
                var result = await ExecuteNodeAsync(stmt);
                if (!result.IsSucces)
                {
                    return result;
                }  
            }
            return ScriptResult.Create();
        }

        private bool EvaluateCondition(ConditionNode cond)
        {
            switch (cond)
            {
                case CompareNode c:
                    var left = _variables.ContainsKey(c.Left) ? _variables[c.Left]?.ToString() : c.Left;
                    var right = _variables.ContainsKey(c.Right) ? _variables[c.Right]?.ToString() : c.Right;
                    return c.Op switch
                    {
                        "=" => left == right,
                        "!=" => left != right,
                        "<" => string.Compare(left, right) < 0,
                        ">" => string.Compare(left, right) > 0,
                        "<=" => string.Compare(left, right) <= 0,
                        ">=" => string.Compare(left, right) >= 0,
                        _ => false
                    };

                case AndNode a:
                    return EvaluateCondition(a.Left) && EvaluateCondition(a.Right);

                case OrNode o:
                    return EvaluateCondition(o.Left) || EvaluateCondition(o.Right);

                case ValueNode v:
                    return !string.IsNullOrEmpty(v.Value);

                default:
                    return false;
            }
        }

        private async Task<object> EvaluateExpressionAsync(ExpressionNode expr)
        {
            switch (expr)
            {
                case StringNode s: return s.Value;
                case VariableNode v:
                    if (_variables.TryGetValue(v.Name, out var val))
                        return val;
                    return ScriptResult.CreateError($"Variable not found: {v.Name}");
                case FunctionCallNode f:
                    var args = new List<object>();
                    foreach (var a in f.Arguments)
                        args.Add(await EvaluateExpressionAsync(a));

                    // Call Cubase function via your API
                    return await _cubase.CallFunctionAsync(f.Name, args.ToArray());

                default:
                    return ScriptResult.CreateError("Unknown Node Type");
            }
        }
    }



}
