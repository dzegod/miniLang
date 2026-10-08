namespace miniLang;

public sealed class Lexer
{

    private static readonly Dictionary<string, TokenType> Keywords;

    static Lexer()
    {
        Keywords = new Dictionary<string, TokenType>
        {
            { "let", TokenType.Let },
            { "print", TokenType.Print },
            { "if", TokenType.If },
            { "else", TokenType.Else },
            { "while", TokenType.While },
            { "true", TokenType.True },
            { "false", TokenType.False }
        };
    }
    
    public static List<Token> Scan(string source)
    {
        var tokens = new List<Token>();
        int i = 0;
        int line = 1;

        while (i < source.Length)
        {
            char currentChar = source[i];

            if (char.IsDigit(currentChar))
            {
                int start = i;
                while (i < source.Length && char.IsDigit(source[i]))
                {
                    i++;
                }
                tokens.Add(new Token(TokenType.Number, source[start..i], line));
            }
            else if (currentChar == '+')
            {
                tokens.Add(new Token(TokenType.Plus, currentChar.ToString(), line));
                i++;
            }
            else if (currentChar == '-')
            {
                tokens.Add(new Token(TokenType.Minus, currentChar.ToString(), line));
                i++;
            }
            else if (currentChar == '*')
            {
                tokens.Add(new Token(TokenType.Star, currentChar.ToString(), line));
                i++;
            }
            else if (currentChar == '/')
            {
                tokens.Add(new Token(TokenType.Slash, currentChar.ToString(), line));
                i++;
            }
            else if (currentChar == '(')
            {
                tokens.Add(new Token(TokenType.LParen, currentChar.ToString(), line));
                i++;
            }
            else if (currentChar == ')')
            {
                tokens.Add(new Token(TokenType.RParen, currentChar.ToString(), line));
                i++;
            }
            else if (currentChar == '=')
            {
                if (i + 1 < source.Length && source[i + 1] == '=')
                {
                    tokens.Add(new Token(TokenType.EqualEqual, "==", line));
                    i += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.Equal, "=", line));
                    i++;
                }
            }
            else if (currentChar == '"')
            {
                int start = i + 1;
                int startLine = line;
                i++;
                while (i < source.Length && source[i] != '"')
                {
                    if (source[i] == '\n')
                    {
                        line++;
                    }
                    i++;
                }
                if (i >= source.Length)
                {
                    throw new LexError("Unterminated string literal", startLine);
                }
                string text = source[start..i];
                tokens.Add(new Token(TokenType.String, text, startLine));
                i++;
            }
            else if (currentChar == ';')
            {
                tokens.Add(new Token(TokenType.Semicolon, currentChar.ToString(), line));
                i++;
            }
            else if (char.IsLetter(currentChar) || currentChar == '_')
            {
                int start = i;
                while (i < source.Length && (char.IsLetterOrDigit(source[i]) || source[i] == '_'))
                {
                    i++;
                }
                string text = source[start..i];
                if (Keywords.TryGetValue(text, out TokenType keywordType))
                {
                    tokens.Add(new Token(keywordType, text, line));
                }
                else
                {
                    tokens.Add(new Token(TokenType.Identifier, text, line));
                }
            }
            else if (currentChar == '<')
            {
                if (i + 1 < source.Length && source[i + 1] == '=')
                {
                    tokens.Add(new Token(TokenType.LessEqual, "<=", line));
                    i += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.Less, "<", line));
                    i++;
                }
            }
            else if (currentChar == '>')
            {
                if (i + 1 < source.Length && source[i + 1] == '=')
                {
                    tokens.Add(new Token(TokenType.GreaterEqual, ">=", line));
                    i += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.Greater, ">", line));
                    i++;
                }
            }
            else if (currentChar == '{')
            {
                tokens.Add(new Token(TokenType.LBrace, currentChar.ToString(), line));
                i++;
            }
            else if (currentChar == '}')
            {
                tokens.Add(new Token(TokenType.RBrace, currentChar.ToString(), line));
                i++;
            }
            else if (currentChar == '\n')
            {
                line++;
                i++;
            }
            else if (char.IsWhiteSpace(currentChar))
            {
                i++;
            }
            else
            {
                throw new LexError($"Unexpected character: '{currentChar}'", line);
            }
        }

        tokens.Add(new Token(TokenType.Eof, "", line));
        return tokens;
    }
}