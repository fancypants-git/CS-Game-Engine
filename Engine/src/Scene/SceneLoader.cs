using System.Diagnostics.CodeAnalysis;
using Engine.Debugging;
using Engine.Internals;

namespace Engine.Scene;

public static class SceneLoader
{
    public struct Token
    {
        public TokenType Type;
        public string Lexeme;
        public int Line;
        public int Column;

        public Token(TokenType type, string value, int line, int column)
        {
            Type = type;
            Lexeme = value;
            Line = line;
            Column = column;
        }
    }

    public enum TokenType
    {
        Identifier,
        String,
        Number,
        Boolean,
        OpenBrace,
        CloseBrace,
        OpenParen,
        CloseParen,
        Separator,
        Linebreak,
        EOF,
        Unknown
    }

    private const char OPEN_BRACE_TOKEN = '{';
    private const char CLOSE_BRACE_TOKEN = '}';
    private const char OPEN_PAREN_TOKEN = '(';
    private const char CLOSE_PAREN_TOKEN = ')';
    private const char STRING_TOKEN = '"';
    private const char SEPARATOR_TOKEN = ',';
    private const char LINEBREAK_TOKEN = '\n';

    private static readonly char[] ALL_SPECIAL_TOKENS = [
        OPEN_BRACE_TOKEN, CLOSE_BRACE_TOKEN,
        OPEN_PAREN_TOKEN, CLOSE_PAREN_TOKEN,
        SEPARATOR_TOKEN, LINEBREAK_TOKEN,
        STRING_TOKEN
    ];

    static SceneLoader()
    {
        blockParsers.Add("entity", new EntityBlockParser());

        commandParsers.Add("default", new DefaultCommandParser());
    }

    private static Dictionary<string, BlockParser> blockParsers = [];
    private static Dictionary<string, CommandParser> commandParsers = [];

    public static void AddBlockParser(string tag, BlockParser parser)
    {
        blockParsers.Add(tag, parser);
    }

    public static void AddCommandParser(string tag, CommandParser parser)
    {
        commandParsers.Add(tag, parser);
    }

    public static CommandParser GetCommandParser(string name)
    {
        if (commandParsers.TryGetValue(name, out CommandParser? parser))
            return parser;

        return commandParsers["default"];
    }


    public static SceneData LoadScene(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogFatal($"Scene File {path} does not exist");
            return new SceneData();
        }

        string content = File.ReadAllText(path);
        Token[] tokens = TokenizeScene(content);

        // foreach (Token t in tokens)
        // {
        //     Debug.Log($"{t.Lexeme} >> {t.Type} {t.Line}:{t.Column}");
        // }
        SceneNode sceneNode = ParseScene(tokens, path);

        string GetIndent(int indent)
        {
            return string.Concat(Enumerable.Repeat("    ", indent)) + "L ";
        }

        void PrintArguments(List<ValueNode> nodes, int indent)
        {
            foreach (ValueNode node in nodes)
            {
                Debug.Log(GetIndent(indent) + node.Value);
                if (node.Type == ValueNode.ValueType.Function)
                {
                    PrintArguments(node.Arguments, indent + 1);
                }
            }
        }

        void PrintChildren(List<CommandNode> nodes, int indent)
        {
            foreach (CommandNode node in nodes)
            {
                Debug.Log(GetIndent(indent) + node.Name);
                PrintArguments(node.Arguments, indent + 1);
            }
        }

        void PrintBlockNodes(List<BlockNode> nodes)
        {
            foreach (BlockNode node in nodes)
            {
                Debug.Log(GetIndent(1) + node.Name);
                Debug.Log(GetIndent(2) + "Arguments:");
                PrintArguments(node.Arguments, 3);
                Debug.Log(GetIndent(2) + "Children:");
                PrintChildren(node.Children, 3);
            }
        }

        Debug.Log(">>> PARSED SCENE");
        Debug.Log("SceneNode:");
        Debug.Log("L MetaNode:");
        Debug.Log("    L " + sceneNode.MetaNode.Name);
        Debug.Log("    L Arguments:");
        PrintArguments(sceneNode.MetaNode.Arguments, 2);
        Debug.Log("    L Children:");
        PrintChildren(sceneNode.MetaNode.Children, 2);
        Debug.Log("L BlockNodes:");
        PrintBlockNodes(sceneNode.BlockNodes);

        SceneMeta meta = new()
        {
            Path = path,
        };
        foreach (CommandNode node in sceneNode.MetaNode.Children)
        {
            if (node.Name == "name")
            {
                meta.Name = node.Arguments[0].GetString();
            }
        }

        SceneData scene = new(meta);

        foreach (BlockNode blockNode in sceneNode.BlockNodes)
        {
            if (!blockParsers.TryGetValue(blockNode.Name, out BlockParser? parser))
            {
                Debug.LogErr($"No parser found for block tag {blockNode.Name}. Be sure to add your parser with AddBlockParser(tag, parser)");
            }

            Entity e = parser!.Parse(blockNode);
            scene.AddEntity(e);
        }


        return new SceneData();
    }


    private static Token[] TokenizeScene(string content)
    {
        List<Token> tokens = [];
        int line = 1;
        int column = 1;
        string text = "";
        int i = 0;

        TokenType Classify(string text)
        {
            if (char.IsDigit(text[0]))
                return TokenType.Number;
            
            if (text == "true" || text == "false")
                return TokenType.Boolean;

            return TokenType.Identifier;
        }

        void Flush()
        {
            if (string.IsNullOrEmpty(text))
                return;
            
            TokenType type = Classify(text);
            tokens.Add(new Token(type, text, line, column));
            column += text.Length;
            i += text.Length - 1;
            text = "";
        }

        string ReadUntill(int start, char flag, bool testSpecial=true)
        {
            int j = start;
            string text = "";
            while (j < content.Length)
            {
                char c = content[j];
                if (c == flag || ALL_SPECIAL_TOKENS.Contains(c) && testSpecial)
                {
                    if (c == '\n' && text[^1] == '\r')
                        text = text[0..^1];
                    break;
                }
                
                text += c;
                j++;
            }

            return text;
        }



        while (i < content.Length)
        {
            char c = content[i];


            if (char.IsWhiteSpace(c))
            {
                Flush();
                column++;

                // dont count in unnecessary newlines
                TokenType lastTokenType = tokens[^1].Type;
                if (c == '\n')
                {
                    if (!(lastTokenType == TokenType.Linebreak || lastTokenType == TokenType.OpenBrace || lastTokenType == TokenType.CloseBrace))
                        tokens.Add(new Token(TokenType.Linebreak, "'\\n'", line, column));
                    line++;
                    column = 1;
                }
                else if (c == '\r' && content[i + 1] == '\n')
                {
                    if (!(lastTokenType == TokenType.Linebreak || lastTokenType == TokenType.OpenBrace || lastTokenType == TokenType.CloseBrace))
                        tokens.Add(new Token(TokenType.Linebreak, "'\\n'", line, column));
                    line++;
                    column = 1;
                    i++; // increment i to compensate for the extra \n character after the \r
                }
            }

            else if (char.IsLetter(c) || c =='_')
            {
                text = ReadUntill(i, ' ');
                Flush();
            }

            else if (char.IsDigit(c))
            {
                text = ReadUntill(i, ' ');
                Flush();
            }

            else if (c == STRING_TOKEN)
            {
                text = ReadUntill(i+1, STRING_TOKEN, false);
                tokens.Add(new Token(TokenType.String, text, line, column));
                column += text.Length + 2;
                i += text.Length + 2;
                text = "";
            }

            else if (c == SEPARATOR_TOKEN)
            {
                Flush();
                column++;
            }

            else if (c == OPEN_BRACE_TOKEN)
            {
                Flush();
                tokens.Add(new Token(TokenType.OpenBrace, OPEN_BRACE_TOKEN.ToString(), line, column));
                column++;
            }
            else if (c == CLOSE_BRACE_TOKEN)
            {
                Flush();
                tokens.Add(new Token(TokenType.CloseBrace, CLOSE_BRACE_TOKEN.ToString(), line, column));
                column++;
            }
            else if (c == OPEN_PAREN_TOKEN)
            {
                Flush();
                tokens.Add(new Token(TokenType.OpenParen, OPEN_PAREN_TOKEN.ToString(), line, column));
                column++;
            }
            else if (c == CLOSE_PAREN_TOKEN)
            {
                Flush();
                tokens.Add(new Token(TokenType.CloseParen, CLOSE_PAREN_TOKEN.ToString(), line, column));
                column++;
            }

            i++;
        }

        tokens.Add(new Token(TokenType.EOF, "'EOF'", line, column));
        return tokens.ToArray();
    }

    private static SceneNode ParseScene(Token[] tokens, string path)
    {
        int position = 0;

        Token Peek() => tokens[position];
        Token Advance() => tokens[position++];

        bool Check(TokenType type)
        {
            if (IsAtEnd())
                return false;
            return Peek().Type == type;
        }

        void Skip() => position++;

        bool SkipIf(TokenType type)
        {
            if (Check(type))
            {
                Skip();
                return true;
            }
            return false;
        }

        void Rewind() => position--;

        bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        BlockNode ParseBlock()
        {
            Token identifierToken = Advance();
            if (identifierToken.Type != TokenType.Identifier)
            {
                Debug.LogFatal($"Error: expected Identifier token, got {identifierToken.Type} \n\tat {path}:{identifierToken.Line}:{identifierToken.Column}");
                return null!;
            }

            BlockNode node = new()
            {
                Name = identifierToken.Lexeme
            };

            while (!SkipIf(TokenType.OpenBrace))
                node.Arguments.Add(ParseValue());
            

            while (!SkipIf(TokenType.CloseBrace))
                node.Children.Add(ParseCommand());

            return node;
        }

        CommandNode ParseCommand()
        {
            CommandNode node = new();

            Token identifierToken = Advance();
            if (identifierToken.Type != TokenType.Identifier)
            {
                Debug.LogFatal($"Error: expected Identifier token, got {identifierToken.Type} \n\tat {path}:{identifierToken.Line}:{identifierToken.Column}");
                return null!;
            }
            
            node.Name = identifierToken.Lexeme;

            while (!SkipIf(TokenType.Linebreak))
            {
                Token token = Peek();
                if (token.Type == TokenType.OpenBrace)
                {
                    Debug.LogWarn($"Warning: Nesting Blocks is not supported \n\tat {path}:{identifierToken.Line}:{identifierToken.Column}");
                    while (!Check(TokenType.CloseBrace)) { Skip(); }
                    SkipIf(TokenType.CloseBrace);
                    return null!;
                }

                node.Arguments.Add(ParseValue());
            }

            return node;
        }

        ValueNode ParseValue()
        {
            Token token = Advance();
            ValueNode node = new()
            {
                Value = token.Lexeme
            };

            if (Check(TokenType.OpenParen))
            {
                Rewind();
                return ParseFunction();
            }

            
            ValueNode.ValueType type = token.Type switch
            {
                TokenType.Identifier => ValueNode.ValueType.Identifier,
                TokenType.String => ValueNode.ValueType.String,
                TokenType.Number => ValueNode.ValueType.Number,
                TokenType.Boolean => ValueNode.ValueType.Boolean,
                _ => throw new Exception()
            };

            node.Type = type;
            
            return node;
        }

        ValueNode ParseFunction()
        {
            Token identifierToken = Advance();

            if (identifierToken.Type != TokenType.Identifier)
            {
                Debug.LogFatal($"Error: expected Identifier token, got {identifierToken.Type} \n    at {path}:{identifierToken.Line}:{identifierToken.Column}");
                return null!;
            }

            if (!SkipIf(TokenType.OpenParen))
            {
                Token faultyToken = Peek();
                Debug.LogFatal($"Error: expected '(' token, got '{faultyToken.Lexeme}'\n\tat {path}:{faultyToken.Line}:{faultyToken.Column}");
                return null!;
            }

            ValueNode node = new()
            {
                Value = identifierToken.Lexeme,
                Type = ValueNode.ValueType.Function
            };

            while (!SkipIf(TokenType.CloseParen))
            {
                node.Arguments.Add(ParseValue());
            }

            return node;
        }


        SceneNode rootNode = new();
        
        while (!IsAtEnd())
        {
            BlockNode node = ParseBlock();

            if (node.Name == "meta")
                rootNode.MetaNode = node;
            else
                rootNode.BlockNodes.Add(node);
        }

        return rootNode;
    }
}


public class SceneNode
{
    public BlockNode MetaNode;
    public List<BlockNode> BlockNodes = new();
}

public class BlockNode
{
    public string Name;
    public List<ValueNode> Arguments = new();
    public List<CommandNode> Children = new();
}

public class CommandNode
{
    public string Name;
    public List<ValueNode> Arguments = new();
}

public class ValueNode
{
    public enum ValueType
    {
        Identifier,
        String,
        Number,
        Boolean,
        Function
    }
    public ValueType Type;
    public required string Value;
    public List<ValueNode> Arguments = new(); // Only used by Function Values to store the arguments of the function


    public int ParseInt()
    {
        if (int.TryParse(Value, out int result))
            return result;

        Debug.LogErr($"Failed to parse {Value} (ValueType: {Type}) as int.");
        return 0;
    }

    public float ParseFloat()
    {
        if (float.TryParse(Value, out float result))
            return result;
        
        Debug.LogErr($"Failed to parse {Value} (ValueType: {Type}) as float.");
        return 0;
    }

    public bool ParseBool()
    {
        return Value == "true";
    }

    public string GetString() => Value;

    public Parameter Parse()
    {

        if (Type == ValueType.Identifier)
            return new Parameter(GetString(), typeof(string));
        if (Type == ValueType.String)
            return new Parameter(GetString(), typeof(string));
        if (Type == ValueType.Boolean)
            return new Parameter(ParseBool(), typeof(bool));
        if (Type == ValueType.Number)
        {
            if (int.TryParse(GetString(), out int i))
                return new Parameter(i, typeof(int));
            if (float.TryParse(GetString(), out float f))
                return new Parameter(f, typeof(float));
        }
        if (Type == ValueType.Function)
        {
            SceneLoader.
        }
    }
}