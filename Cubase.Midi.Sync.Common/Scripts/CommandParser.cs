using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubase.Midi.Sync.Common.Scripts
{
    // ------------------ EXCEPTIONS ----------------------
    public class ScriptParseException : Exception
    {
        public int LineNumber { get; }
        public string LineText { get; }

        public ScriptParseException(string message, int lineNumber, string line)
            : base($"{message} (line {lineNumber}: '{line}')")
        {
            LineNumber = lineNumber;
            LineText = line;
        }
    }

    // ------------------ AST NODES -------------------------
    public abstract class Node { }

    public class ScriptNode
    {
        public List<Node> Statements { get; set; } = new();
    }

    public class CommandNode : Node
    {
        public string Command { get; set; }
        public List<ExpressionNode> Args { get; set; } = new();
    }

    public class LetNode : Node
    {
        public string Variable { get; set; }
        public ExpressionNode Expression { get; set; }
    }

    public class IfNode : Node
    {
        public ConditionNode Condition { get; set; }
        public List<Node> Then { get; set; } = new();
        public List<Node> Else { get; set; } = new();
    }

    public class ForEachNode : Node
    {
        public string Variable { get; set; }
        public ExpressionNode Collection { get; set; }
        public List<Node> Body { get; set; } = new();
    }

    // ------------------ EXPRESSION NODES -----------------
    public abstract class ExpressionNode { }

    public class StringNode : ExpressionNode
    {
        public string Value { get; set; }
    }

    public class VariableNode : ExpressionNode
    {
        public string Name { get; set; }
    }

    public class FunctionCallNode : ExpressionNode
    {
        public string Name { get; set; }
        public List<ExpressionNode> Arguments { get; set; } = new();
    }

    // ------------------ CONDITION NODES -----------------
    public abstract class ConditionNode { }

    public class CompareNode : ConditionNode
    {
        public string Left { get; set; }
        public string Op { get; set; }
        public string Right { get; set; }
    }

    public class AndNode : ConditionNode
    {
        public ConditionNode Left { get; set; }
        public ConditionNode Right { get; set; }
    }

    public class OrNode : ConditionNode
    {
        public ConditionNode Left { get; set; }
        public ConditionNode Right { get; set; }
    }

    public class ValueNode : ConditionNode
    {
        public string Value { get; set; }
    }

    public class CommandConditionNode : ConditionNode
    {
        public string Command { get; set; }
        public List<string> Args { get; set; } = new();
    }

    // ------------------ PARSER ----------------------------
    public class ScriptParser
    {
        private List<List<string>> _tokenLines;
        private int _lineIndex;

        public ScriptNode Parse(IEnumerable<string> lines, Action<ScriptParseException> errorHandler)
        {
            lines = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            _tokenLines = lines.Select(TokenizeLine).ToList();
            _lineIndex = 0;

            return new ScriptNode
            {
                Statements = ParseStatements(errorHandler)
            };
        }

        private List<Node> ParseStatements(Action<ScriptParseException> errorHandler)
        {
            var list = new List<Node>();
            while (!End)
            {
                var token = PeekToken(0);
                if (token.Equals("endif", StringComparison.OrdinalIgnoreCase) ||
                    token.Equals("else", StringComparison.OrdinalIgnoreCase) ||
                    token.Equals("endforeach", StringComparison.OrdinalIgnoreCase))
                    break;

                try
                {
                    if (token.Equals("let", StringComparison.OrdinalIgnoreCase))
                        list.Add(ParseLet(errorHandler));
                    else if (token.Equals("if", StringComparison.OrdinalIgnoreCase))
                        list.Add(ParseIf(errorHandler));
                    else if (token.Equals("foreach", StringComparison.OrdinalIgnoreCase))
                        list.Add(ParseForEach(errorHandler));
                    else
                        list.Add(ParseCommand(errorHandler));
                }
                catch (ScriptParseException ex)
                {
                    errorHandler.Invoke(ex);
                }
            }
            return list;
        }

        // ----------------- LET -----------------------------
        private Node ParseLet(Action<ScriptParseException> errorHandler)
        {
            var line = ConsumeLine();
            if (line.Count < 4 || line[2] != "=")
                throw new ScriptParseException("Invalid let statement", _lineIndex, string.Join(" ", line));

            string varName = line[1];
            var exprTokens = line.Skip(3).ToList();
            if (exprTokens.Count == 0)
                throw new ScriptParseException("Missing expression", _lineIndex, string.Join(" ", line));

            return new LetNode
            {
                Variable = varName,
                Expression = ParseExpressionTokens(exprTokens, errorHandler)
            };
        }

        // ----------------- COMMAND -------------------------
        private Node ParseCommand(Action<ScriptParseException> errorHandler)
        {
            var line = ConsumeLine();
            if (line.Count == 0) return null;

            // If the command is actually a function call:
            // e.g. DisableRecord("all")
            if (line.Count > 1 && line[1] == "(")
            {
                var expr = ParseExpressionTokens(line, errorHandler);

                if (expr is FunctionCallNode fn)
                {
                    return new CommandNode
                    {
                        Command = fn.Name,
                        Args = fn.Arguments
                    };
                }

                throw new ScriptParseException("Invalid function-call syntax", _lineIndex - 1, string.Join(" ", line));
            }

            // Regular command with normal arguments (no parentheses)
            // e.g. SelectTrack Vocal 1
            var argsExpr = ParseExpressionTokens(line.Skip(1).ToList(), errorHandler);
            var args = argsExpr?.ToFunctionArgList() ?? new List<ExpressionNode>();

            return new CommandNode
            {
                Command = line[0],
                Args = args
            };
        }

        // ----------------- IF ------------------------------
        private Node ParseIf(Action<ScriptParseException> errorHandler)
        {
            var line = ConsumeLine();
            int startLine = _lineIndex;

            if (!line[0].Equals("if", StringComparison.OrdinalIgnoreCase))
                throw new ScriptParseException("Expected 'if'", startLine, string.Join(" ", line));

            int thenIndex = line.FindIndex(t => t.Equals("then", StringComparison.OrdinalIgnoreCase));
            if (thenIndex == -1)
                throw new ScriptParseException("Missing 'then' in if statement", startLine, string.Join(" ", line));

            var condTokens = line.Skip(1).Take(thenIndex - 1).ToList();
            var condition = ParseConditionTokens(condTokens);

            var afterThen = line.Skip(thenIndex + 1).ToList();
            List<Node> thenBlock, elseBlock = null;

            if (afterThen.Count > 0)
            {
                thenBlock = new List<Node> { MakeInlineCommand(afterThen, errorHandler) };
                return new IfNode
                {
                    Condition = condition,
                    Then = thenBlock,
                    Else = null
                };
            }

            thenBlock = ParseStatements(errorHandler);

            if (PeekToken(0).Equals("else", StringComparison.OrdinalIgnoreCase))
            {
                ConsumeLine(); // skip else
                elseBlock = ParseStatements(errorHandler);
            }

            if (!PeekToken(0).Equals("endif", StringComparison.OrdinalIgnoreCase))
                throw new ScriptParseException("Expected 'endif'", _lineIndex, "");

            ConsumeLine(); // skip endif

            return new IfNode
            {
                Condition = condition,
                Then = thenBlock,
                Else = elseBlock
            };
        }

        private CommandNode MakeInlineCommand(List<string> tokens, Action<ScriptParseException> errorHandler)
        {
            return new CommandNode
            {
                Command = tokens[0],
                Args = ParseExpressionTokens(tokens.Skip(1).ToList(), errorHandler).ToFunctionArgList()
            };
        }

        // ----------------- FOREACH -------------------------
        /*
        private Node ParseForEach(Action<ScriptParseException> errorHandler)
        {
            var line = ConsumeLine();
            if (line.Count < 5 || line[1] != "(" || !line.Contains("in") || line[^1] != ")")
                throw new ScriptParseException("Invalid foreach syntax", _lineIndex, string.Join(" ", line));

            int inIndex = line.IndexOf("in");
            string varName = line[2]; // e.g., $currentTrack
            var exprTokens = line.Skip(inIndex + 1).Take(line.Count - inIndex - 2).ToList();

            var collectionExpr = ParseExpressionTokens(exprTokens, errorHandler);

            var body = ParseStatements(errorHandler);

            if (!PeekToken(0).Equals("endforeach", StringComparison.OrdinalIgnoreCase))
                throw new ScriptParseException("Expected 'endforeach'", _lineIndex, "");

            ConsumeLine(); // skip endforeach

            return new ForEachNode
            {
                Variable = varName,
                Collection = collectionExpr,
                Body = body
            };
        }
        */
        private Node ParseForEach(Action<ScriptParseException> errorHandler)
        {
            var line = ConsumeLine();

            if (line.Count < 5 || line[1] != "(" || !line.Contains("in") || line[^1] != ")")
                throw new ScriptParseException("Invalid foreach syntax", _lineIndex, string.Join(" ", line));

            int inIndex = line.IndexOf("in");

            // Strip $ prefix here so variable names match AST VariableNode
            string varName = line[2].TrimStart('$');

            var exprTokens = line.Skip(inIndex + 1).Take(line.Count - inIndex - 2).ToList();
            var collectionExpr = ParseExpressionTokens(exprTokens, errorHandler);

            var body = ParseStatements(errorHandler);

            if (!PeekToken(0).Equals("endforeach", StringComparison.OrdinalIgnoreCase))
                throw new ScriptParseException("Expected 'endforeach'", _lineIndex, "");

            ConsumeLine(); // skip endforeach

            return new ForEachNode
            {
                Variable = varName,
                Collection = collectionExpr,
                Body = body
            };
        }

        // ----------------- EXPRESSIONS ----------------------
        private ExpressionNode ParseExpressionTokens(List<string> tokens, Action<ScriptParseException> errorHandler)
        {
            if (tokens == null || tokens.Count == 0) return null;

            // function style: name ( ... )
            if (tokens.Count > 1 && tokens[1] == "(")
                return ParseFunctionCall(tokens, errorHandler);

            // multi-token without parentheses — treat as function-ish where first token is name and rest are simple args
            if (tokens.Count > 1)
            {
                string funcName = tokens[0];
                var args = tokens.Skip(1)
                                 .Select(ParseSingleToken)
                                 .Where(n => n != null)
                                 .ToList();
                return new FunctionCallNode
                {
                    Name = funcName,
                    Arguments = args
                };
            }

            // single token
            return ParseSingleToken(tokens[0]);
        }

        private ExpressionNode ParseFunctionCall(List<string> tokens, Action<ScriptParseException> errorHandler)
        {
            string funcName = tokens[0];
            var args = new List<ExpressionNode>();
            int i = 2; // start after name and '('

            while (i < tokens.Count && tokens[i] != ")")
            {
                string token = tokens[i];

                if (token == ",") { i++; continue; }

                // nested function call starting at this token (token is name and next is '(')
                if (i + 1 < tokens.Count && tokens[i + 1] == "(")
                {
                    var nestedTokens = CollectTokensForNestedFunction(tokens, i);
                    args.Add(ParseFunctionCall(nestedTokens, errorHandler));
                    i += nestedTokens.Count;
                    continue;
                }

                var single = ParseSingleToken(token);
                if (single != null)
                    args.Add(single);
                i++;
            }

            if (i >= tokens.Count || tokens[i] != ")")
                throw new ScriptParseException("Expected closing ')'", _lineIndex, string.Join(" ", tokens));

            // consume the closing ')'
            i++;

            return new FunctionCallNode
            {
                Name = funcName,
                Arguments = args
            };
        }

        private List<string> CollectTokensForNestedFunction(List<string> tokens, int start)
        {
            var collected = new List<string>();
            int depth = 0;
            for (int i = start; i < tokens.Count; i++)
            {
                collected.Add(tokens[i]);
                if (tokens[i] == "(") depth++;
                if (tokens[i] == ")") depth--;
                if (depth == 0) break;
            }
            return collected;
        }

        private ExpressionNode ParseSingleToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            token = token.Trim();

            // ignore punctuation tokens generated by TokenizeLine
            if (token == "(" || token == ")" || token == ",")
                return null;

            // quoted strings (token will include quotes)
            if ((token.StartsWith('"') && token.EndsWith('"')) ||
                (token.StartsWith('\'') && token.EndsWith('\'')))
            {
                return new StringNode { Value = token.Substring(1, token.Length - 2) };
            }

            // variables with $ prefix
            if (token.StartsWith("$"))
            {
                return new VariableNode { Name = token.Substring(1) };
            }

            // unquoted identifiers → treat as variable/identifier (not a literal string)
            return new VariableNode { Name = token };
        }


        // ----------------- CONDITIONS ----------------------
        private ConditionNode ParseConditionTokens(List<string> tokens)
        {
            var tk = new TokenStream(tokens);
            return ParseOr(tk);
        }

        private ConditionNode ParseOr(TokenStream t)
        {
            var left = ParseAnd(t);
            while (t.Match("or"))
            {
                var right = ParseAnd(t);
                left = new OrNode { Left = left, Right = right };
            }
            return left;
        }

        private ConditionNode ParseAnd(TokenStream t)
        {
            var left = ParseCompare(t);
            while (t.Match("and"))
            {
                var right = ParseCompare(t);
                left = new AndNode { Left = left, Right = right };
            }
            return left;
        }

        private ConditionNode ParseCompare(TokenStream t)
        {
            var first = t.RequireValue();
            if (t.MatchAny("=", "!=", "<", ">", "<=", ">="))
            {
                string op = t.LastMatch;
                string right = t.RequireValue();
                return new CompareNode { Left = first, Op = op, Right = right };
            }

            if (t.HasMore)
            {
                var args = new List<string>();
                while (t.HasMore) args.Add(t.RequireValue());
                return new CommandConditionNode { Command = first, Args = args };
            }

            return new ValueNode { Value = first };
        }

        // ----------------- UTILITIES -----------------------
        private bool End => _lineIndex >= _tokenLines.Count;

        private List<string> ConsumeLine()
        {
            if (End) throw new ScriptParseException("Unexpected end of script", _lineIndex, "");
            return _tokenLines[_lineIndex++];
        }

        private string PeekToken(int pos)
        {
            if (End) return "";
            var line = _tokenLines[_lineIndex];
            return pos < line.Count ? line[pos] : "";
        }

        private List<string> TokenizeLine(string line)
        {
            var tokens = new List<string>();
            var buffer = new List<char>();
            bool inQuotes = false;
            char quoteChar = '"';

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if ((c == '"' || c == '\''))
                {
                    if (!inQuotes)
                    {
                        // start quoted string - include opening quote
                        inQuotes = true;
                        quoteChar = c;
                        buffer.Add(c);
                        continue;
                    }
                    else if (c == quoteChar)
                    {
                        // include closing quote then flush
                        buffer.Add(c);
                        inQuotes = false;
                        tokens.Add(new string(buffer.ToArray()));
                        buffer.Clear();
                        continue;
                    }
                }

                if (!inQuotes && (c == '(' || c == ')' || c == ','))
                {
                    if (buffer.Count > 0)
                    {
                        tokens.Add(new string(buffer.ToArray()));
                        buffer.Clear();
                    }
                    tokens.Add(c.ToString());
                    continue;
                }

                if (!inQuotes && char.IsWhiteSpace(c))
                {
                    if (buffer.Count > 0)
                    {
                        tokens.Add(new string(buffer.ToArray()));
                        buffer.Clear();
                    }
                    continue;
                }

                buffer.Add(c);
            }

            if (buffer.Count > 0)
                tokens.Add(new string(buffer.ToArray()));

            return tokens;
        }

        private class TokenStream
        {
            private readonly List<string> tokens;
            private int index;
            public string LastMatch { get; private set; }
            public bool HasMore => index < tokens.Count;

            public TokenStream(List<string> tokens)
            {
                this.tokens = tokens;
                index = 0;
            }

            public bool Match(string t)
            {
                if (index < tokens.Count && tokens[index].Equals(t, StringComparison.OrdinalIgnoreCase))
                {
                    LastMatch = t;
                    index++;
                    return true;
                }
                return false;
            }

            public bool MatchAny(params string[] ops)
            {
                foreach (var op in ops)
                    if (Match(op)) return true;
                return false;
            }

            public string RequireValue()
            {
                if (index >= tokens.Count)
                    throw new Exception("Expected value in condition");
                return tokens[index++];
            }
        }
    }

    public static class ExpressionNodeExtensions
    {
        public static List<ExpressionNode> ToFunctionArgList(this ExpressionNode node)
        {
            if (node == null) return new List<ExpressionNode>();
            if (node is FunctionCallNode fn) return fn.Arguments;
            return new List<ExpressionNode> { node };
        }
    }
}
