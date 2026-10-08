namespace miniLang;

public class Lexer
{

    private static readonly Dictionary<string, TokenType> Keywords = new()
    
    {
        { "let", TokenType.Let },
        { "print", TokenType.Print },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "true", TokenType.True },
        { "false", TokenType.False }
    };
    public static List<Token> Scan(string source)
    {
        var tokens = new List<Token>();
        int i = 0;

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
                tokens.Add(new Token(TokenType.Number, source[start..i]));
            }
            else if (currentChar == '+')
            {
                tokens.Add(new Token(TokenType.Plus, currentChar.ToString()));
                i++;
            }
            else if (currentChar == '-')
            {
                tokens.Add(new Token(TokenType.Minus, currentChar.ToString()));
                i++;
            }
            else if (currentChar == '*')
            {
                tokens.Add(new Token(TokenType.Star, currentChar.ToString()));
                i++;
            }
            else if (currentChar == '/')
            {
                tokens.Add(new Token(TokenType.Slash, currentChar.ToString()));
                i++;
            }
            else if (currentChar == '(')
            {
                tokens.Add(new Token(TokenType.LParen, currentChar.ToString()));
                i++;
            }
            else if (currentChar == ')')
            {
                tokens.Add(new Token(TokenType.RParen, currentChar.ToString()));
                i++;
            }
            else if (currentChar == '=')
            {
                if (i + 1 < source.Length && source[i + 1] == '=')
                {
                    tokens.Add(new Token(TokenType.EqualEqual, "=="));
                    i += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.Equal, "="));
                    i++;
                }
            }
            else if (currentChar == '"')
            {
                int start = i + 1;
                i++;
                while (i < source.Length && source[i] != '"')
                {
                    i++;
                }
                if (i >= source.Length)
                {
                    throw new LexError("Unterminated string literal");
                }
                string text = source[start..i];
                tokens.Add(new Token(TokenType.String, text));
                i++;
            }
            else if (currentChar == ';')
            {
                tokens.Add(new Token(TokenType.Semicolon, currentChar.ToString()));
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
                    tokens.Add(new Token(keywordType, text));
                }
                else
                {
                    tokens.Add(new Token(TokenType.Identifier, text));
                }
            }
            else if (currentChar == '<')
            {
                if (i + 1 < source.Length && source[i + 1] == '=')
                {
                    tokens.Add(new Token(TokenType.LessEqual, "<="));
                    i += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.Less, "<"));
                    i++;
                }
            }
            else if (currentChar == '>')
            {
                if (i + 1 < source.Length && source[i + 1] == '=')
                {
                    tokens.Add(new Token(TokenType.GreaterEqual, ">="));
                    i += 2;
                }
                else
                {
                    tokens.Add(new Token(TokenType.Greater, ">"));
                    i++;
                }
            }
            else if (currentChar == '{')
            {
                tokens.Add(new Token(TokenType.LBrace, currentChar.ToString()));
                i++;
            }
            else if (currentChar == '}')
            {
                tokens.Add(new Token(TokenType.RBrace, currentChar.ToString()));
                i++;
            }
            else if (char.IsWhiteSpace(currentChar))
            {
                i++;
            }
            else
            {
                throw new LexError($"Unexpected character: '{currentChar}'");
            }
        }

        tokens.Add(new Token(TokenType.Eof, ""));
        return tokens;
    }
}