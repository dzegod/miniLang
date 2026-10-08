namespace miniLang;

public sealed class Parser
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
                throw Error("Expected closing parenthesis");
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
        if (token.Type == TokenType.String)
        {
            return new StringExpression(token.Lexeme);
        }
        throw new ParseError($"Unexpected token: {token.Type}", token.Line);
    }

    private Token Peek() => _pos < _tokens.Count ? _tokens[_pos] : _tokens[^1];

    private Token Advance()
    {
        var token = Peek();
        if (token.Type != TokenType.Eof)
        {
            _pos++;
        }
        return token;
    }

    private bool Check(TokenType type) => Peek().Type == type;

    // Creates a ParseError that points at the line of the current token.
    private ParseError Error(string message) => new ParseError(message, Peek().Line);

    public List<Statement> ParseProgram()
    {
        var statements = new List<Statement>();
        while (_pos < _tokens.Count && !Check(TokenType.Eof))
        {
            statements.Add(ParseStatement());
        }
        return statements;
    }

    // Remembers the line a statement starts on, so runtime errors can report it.
    public Statement ParseStatement()
    {
        var line = Peek().Line;
        return ParseStatementKind() with { Line = line };
    }

    private Statement ParseStatementKind()
    {
        if(Check(TokenType.Let))
        {
            Advance();
            if (!Check(TokenType.Identifier))
            {
                throw Error("Expected variable name after 'let'");
            }
            var name = Advance().Lexeme;

            if(!Check(TokenType.Equal))
            {
                throw Error("Expected '=' after variable name");
            }
            Advance();
            var value = ParseExpression();

            if(!Check(TokenType.Semicolon))
            {
                throw Error("Expected ';' after expression");
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
                throw Error("Expected ';' after expression");
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
                throw Error("Expected '}' after block");
            }
            Advance();
            return new BlockStatement(statements);
        }

        if (Check(TokenType.If))
        {
            Advance();

            if (!Check(TokenType.LParen))
            {
                throw Error("Expected '(' after 'if'");
            }
            Advance();

            var condition = ParseExpression();

            if (!Check(TokenType.RParen))
            {
                throw Error("Expected ')' after condition");
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
        if (Check(TokenType.While))
        {
            return ParseWhile();
        }

        throw Error($"Unexpected token: {Peek().Type}");
    }

    private Statement ParseAssignment()
    {
        if (!Check(TokenType.Identifier))
        {
            throw Error("Expected variable name for assignment");
        }
        var name = Advance().Lexeme;

        if (!Check(TokenType.Equal))
        {
            throw Error("Expected '=' after variable name");
        }
        Advance();
        var value = ParseExpression();

        if (!Check(TokenType.Semicolon))
        {
            throw Error("Expected ';' after expression");
        }
        Advance();

        return new AssignmentStatement(name, value);
    }

    private Statement ParseWhile()
    {
        Advance();

        if (!Check(TokenType.LParen))
        {
            throw Error("Expected '(' after 'while'");
        }
        Advance();

        var condition = ParseExpression();

        if (!Check(TokenType.RParen))
        {
            throw Error("Expected ')' after condition");
        }
        Advance();

        var body = ParseStatement();

        return new WhileStatement(condition, body);
    } 
}