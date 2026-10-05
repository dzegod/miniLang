namespace miniLang;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _pos = 0;

    public Parser (List<Token> tokens)
    {
        _tokens = tokens;
    }

    public Expression ParseExpression()
    {
        var left = ParseTerm();

        while (Check(TokenType.Plus) || Check(TokenType.Minus))
        {
            var op = Advance().Type;
            var right = ParseTerm();
            left = new BinaryExpression(left, op, right);
        }
        return left;
    }

    private Expression ParseTerm()
    {
        var left = ParsePrimary();

        while(Check(TokenType.Star) || Check(TokenType.Slash))
        {
            var op = Advance().Type;
            var right = ParsePrimary();
            left = new BinaryExpression(left, op, right);
        }
        return left;
    }

    private Expression ParsePrimary()
    {
        if (Check(TokenType.LParen))
        {
            Advance();
            var expr = ParseExpression();

            if (!Check(TokenType.RParen))
            {
                throw new Exception("Expected closing parenthesis");
            }
            Advance();
            return expr;
        }

        var token = Advance();
        if (token.Type == TokenType.Number)
        {
            return new NumberExpression(double.Parse(token.Lexeme));
        }
        throw new Exception($"Unexpected token: {token.Type}");
    }

    private Token Advance() => _tokens[_pos++];

    private bool Check(TokenType type) => _pos < _tokens.Count && _tokens[_pos].Type == type;

    public Statement ParseStatement()
    {
        if(Check(TokenType.Let))
        {
            Advance();
            var name = Advance().Lexeme;

            if(!Check(TokenType.Equal))
            {
                throw new Exception("Expected '=' after variable name");
            }
            Advance();
            var value = ParseExpression();

            if(!Check(TokenType.Semicolon))
            {
                throw new Exception("Expected ';' after expression");
            }
            Advance();

            return new LetStatement(name, value);
        }

    }
}