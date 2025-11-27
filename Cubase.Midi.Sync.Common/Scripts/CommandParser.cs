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
        public List<string> Args { get; set; } = new();
    }

    public class IfNode : Node
    {
        public ConditionNode Condition { get; set; }
        public List<Node> Then { get; set; }
        public List<Node> Else { get; set; }
    }

    // ------------------ CONDITION NODES -------------------

    public abstract class ConditionNode { }

    public class CompareNode : ConditionNode
    {
        public string Left { get; set; }
        public string Op { get; set; } // = != < > <= >=
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
        public List<string> Args { get; set; }
    }

    // ------------------ PARSER ----------------------------

    public class CommandParser
    {
        private List<List<string>> _tokenLines;
        private int _lineIndex;

        public ScriptNode Parse(IEnumerable<string> lines, Action<ScriptParseException> errorHandler)
        {
            lines = lines.RemoveBlankLines();
            _tokenLines = lines.Select(Tokenize).ToList();
            _lineIndex = 0;

            return new ScriptNode
            {
                Statements = ParseStatements(errorHandler)
            };
        }

        // STATEMENT LIST ------------------------------------------

        private List<Node> ParseStatements(Action<ScriptParseException> errorHandler)
        {
            List<Node> list = new();

            while (!End)
            {
                if (PeekToken(0).Equals("endif", StringComparison.OrdinalIgnoreCase))
                    break;

                var token = PeekToken(0);

                if (token.Equals("if", StringComparison.OrdinalIgnoreCase))
                    list.Add(ParseIf(errorHandler));
                else
                    list.Add(ParseCommand(errorHandler));
            }

            return list;
        }

        private Node ParseCommand(Action<ScriptParseException> errorHandler)
        {
            var tokens = ConsumeLine(errorHandler);
            return new CommandNode
            {
                Command = tokens[0],
                Args = tokens.Skip(1).ToList()
            };
        }

        // IF --------------------------------------------------------

        private Node ParseIf(Action<ScriptParseException> errorHandler)
        {
            var line = ConsumeLine(errorHandler);
            int startLine = _lineIndex;

            // expect: if <condition> then
            if (!line[0].Equals("if", StringComparison.OrdinalIgnoreCase))
                Throw("Expected 'if'", errorHandler);

            // parse condition (tokens between "if" and "then")
            int thenIndex = line.FindIndex(t => t.Equals("then", StringComparison.OrdinalIgnoreCase));
            if (thenIndex == -1)
                Throw("Missing 'then' in if statement", errorHandler, startLine - 1);

            var condTokens = line.Skip(1).Take(thenIndex - 1).ToList();
            var condition = ParseConditionTokens(condTokens);

            // AFTER then → check single-line IF
            var afterThen = line.Skip(thenIndex + 1).ToList();

            List<Node> thenBlock;
            List<Node> elseBlock = null;

            if (afterThen.Count > 0)
            {
                // inline single-line THEN
                thenBlock = new List<Node>();

                if (afterThen[0].Equals("endif", StringComparison.OrdinalIgnoreCase))
                {
                    return new IfNode
                    {
                        Condition = condition,
                        Then = new(),
                        Else = null
                    };
                }

                if (afterThen[0].Equals("else", StringComparison.OrdinalIgnoreCase))
                {
                    // inline ELSE but THEN is empty
                    thenBlock = new();
                    afterThen.RemoveAt(0);

                    if (afterThen.Count == 0)
                        Throw("Missing else command in single-line if", errorHandler);

                    if (afterThen[0].Equals("endif", StringComparison.OrdinalIgnoreCase))
                    {
                        elseBlock = new();
                        return new IfNode
                        {
                            Condition = condition,
                            Then = thenBlock,
                            Else = elseBlock
                        };
                    }

                    elseBlock = new() { MakeInlineCommand(afterThen) };
                }
                else
                {
                    // inline THEN command
                    thenBlock.Add(MakeInlineCommand(afterThen));
                }

                // Expect end-of-if on next line
                ExpectToken("endif", errorHandler);
                ConsumeLine(errorHandler);

                return new IfNode
                {
                    Condition = condition,
                    Then = thenBlock,
                    Else = elseBlock
                };
            }

            // MULTI-LINE THEN BLOCK
            thenBlock = ParseStatements(errorHandler);

            // else or endif
            if (PeekToken(0).Equals("else", StringComparison.OrdinalIgnoreCase))
            {
                ConsumeLine(errorHandler); // skip else
                elseBlock = ParseStatements(errorHandler);
            }

            ExpectToken("endif", errorHandler);
            ConsumeLine(errorHandler); // skip endif

            return new IfNode
            {
                Condition = condition,
                Then = thenBlock,
                Else = elseBlock
            };
        }

        private CommandNode MakeInlineCommand(List<string> tokens)
        {
            return new CommandNode
            {
                Command = tokens[0],
                Args = tokens.Skip(1).ToList()
            };
        }

        // ----------------- CONDITION PARSER ------------------------

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

            // If next token is a comparison operator -> normal comparison
            if (t.MatchAny("=", "!=", "<", ">", "<=", ">="))
            {
                string op = t.LastMatch;
                string right = t.RequireValue();

                return new CompareNode
                {
                    Left = first,
                    Op = op,
                    Right = right
                };
            }

            // 🎯 NEW: treat remaining tokens as arguments to a command condition
            if (t.HasMore)
            {
                var args = new List<string>();

                while (t.HasMore)
                    args.Add(t.RequireValue());

                return new CommandConditionNode
                {
                    Command = first,
                    Args = args
                };
            }

            return new ValueNode { Value = first };
        }


        // ---------------- TOKEN UTILITIES --------------------------

        private bool End => _lineIndex >= _tokenLines.Count;

        private List<string> ConsumeLine(Action<ScriptParseException> errorHandler)
        {
            if (End)
                Throw("Unexpected end of script", errorHandler);

            return _tokenLines[_lineIndex++];
        }

        private string PeekToken(int pos)
        {
            if (End) return "";
            var line = _tokenLines[_lineIndex];
            return (pos < line.Count ? line[pos] : "");
        }

        private void ExpectToken(string t, Action<ScriptParseException> errorHandler)
        {
            if (!PeekToken(0).Equals(t, StringComparison.OrdinalIgnoreCase))
                Throw($"Expected '{t}'", errorHandler);
        }

        private void Throw(string msg, Action<ScriptParseException> errorHandler, int? line = null)
        {
            int idx = line ?? _lineIndex;
            errorHandler.Invoke(new ScriptParseException(msg, idx + 1, idx < _tokenLines.Count ? string.Join(" ", _tokenLines[idx]) : ""));
        }

        // ----------- TOKENIZER ------------------------------------

        private List<string> Tokenize(string line)
        {
            List<string> result = new();
            bool inQuotes = false;
            var buffer = new List<char>();

            foreach (char c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && Char.IsWhiteSpace(c))
                {
                    if (buffer.Count > 0)
                    {
                        result.Add(new string(buffer.ToArray()));
                        buffer.Clear();
                    }
                }
                else
                {
                    buffer.Add(c);
                }
            }

            if (buffer.Count > 0)
                result.Add(new string(buffer.ToArray()));

            return result;
        }

        // ============= TOKEN STREAM (for conditions) ===============

        private class TokenStream
        {
            public bool HasMore => index < tokens.Count;
            private readonly List<string> tokens;
            private int index;

            public string LastMatch { get; private set; }

            public TokenStream(List<string> tokens)
            {
                this.tokens = tokens;
                this.index = 0;
            }

            public bool Match(string t)
            {
                if (index < tokens.Count &&
                    tokens[index].Equals(t, StringComparison.OrdinalIgnoreCase))
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
                {
                    if (Match(op))
                        return true;
                }
                return false;
            }

            public string RequireValue()
            {
                if (index >= tokens.Count)
                    throw new Exception("Expected value");

                string v = tokens[index++];
                return v;
            }
        }
    }
}
