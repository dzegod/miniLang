namespace miniLang;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _pos = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public Expression ParseExpression()
    {
        return ParseComparison();
    }

    private Expression ParseComparison()
    {
        var left = ParseAddition();

        while (Check(TokenType.Less) || Check(TokenType.Greater) || Check(TokenType.LessEqual) || Check(TokenType.GreaterEqual) || Check(TokenType.EqualEqual))
        {
            var op = Advance().Type;
            var right = ParseAddition();
            left = new BinaryExpression(left, op, right);
        }
        return left;
    }
    private Expression ParseAddition()
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
                throw new ParseError("Expected closing parenthesis");
            }
            Advance();
            return expr;
        }

        var token = Advance();
        if (token.Type == TokenType.Number)
        {
            return new NumberExpression(double.Parse(token.Lexeme));
        }
        if (token.Type == TokenType.Identifier)
        {
            return new VariableExpression(token.Lexeme);
        }
        throw new ParseError($"Unexpected token: {token.Type}");
    }

    private Token Advance() => _tokens[_pos++];

    private bool Check(TokenType type) => _pos < _tokens.Count && _tokens[_pos].Type == type;

    public List<Statement> ParseProgram()
    {
        var statements = new List<Statement>();
        while (_pos < _tokens.Count && !Check(TokenType.Eof))
        {
            statements.Add(ParseStatement());
        }
        return statements;
    }

    public Statement ParseStatement()
    {
        if(Check(TokenType.Let))
        {
            Advance();
            if (!Check(TokenType.Identifier))
            {
                throw new ParseError("Expected variable name after 'let'");
            }
            var name = Advance().Lexeme;

            if(!Check(TokenType.Equal))
            {
                throw new ParseError("Expected '=' after variable name");
            }
            Advance();
            var value = ParseExpression();

            if(!Check(TokenType.Semicolon))
            {
                throw new ParseError("Expected ';' after expression");
            }
            Advance();

            return new LetStatement(name, value);
        }

        if (Check(TokenType.Print))
        {
            Advance();
            var value = ParseExpression();

            if (!Check(TokenType.Semicolon))
            {
                throw new ParseError("Expected ';' after expression");
            }
            Advance();

            return new PrintStatement(value);
        }
        if (Check(TokenType.LBrace))
        {
            Advance();
            var statements = new List<Statement>();
            while (!Check(TokenType.RBrace) && !Check(TokenType.Eof))
            {
                statements.Add(ParseStatement());
            }
            if (!Check(TokenType.RBrace))
            {
                throw new ParseError("Expected '}' after block");
            }
            Advance();
            return new BlockStatement(statements);
        }

        if (Check(TokenType.If))
        {
            Advance();

            if (!Check(TokenType.LParen))
            {
                throw new ParseError("Expected '(' after 'if'");
            }
            Advance();

            var condition = ParseExpression();

            if (!Check(TokenType.RParen))
            {
                throw new ParseError("Expected ')' after condition");
            }
            Advance();

            var thenBranch = ParseStatement();

            Statement? elseBranch = null;
            if (Check(TokenType.Else))
            {
                Advance();
                elseBranch = ParseStatement();
            }

            return new IfStatement(condition, thenBranch, elseBranch);
        }

        if (Check(TokenType.Identifier))
        {
            return ParseAssignment();
        }

        throw new ParseError($"Unexpected token: {(_pos < _tokens.Count ? _tokens[_pos].Type : TokenType.Eof)}");
    }

    private Statement ParseAssignment()
    {
        if (!Check(TokenType.Identifier))
        {
            throw new ParseError("Expected variable name for assignment");
        }
        var name = Advance().Lexeme;

        if (!Check(TokenType.Equal))
        {
            throw new ParseError("Expected '=' after variable name");
        }
        Advance();
        var value = ParseExpression();

        if (!Check(TokenType.Semicolon))
        {
            throw new ParseError("Expected ';' after expression");
        }
        Advance();

        return new AssignmentStatement(name, value);
    }
}