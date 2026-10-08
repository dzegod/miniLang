using miniLang;

namespace miniLang.Tests;

public class ParserTests
{
    private static Expression ParseExpr(string source) => new Parser(Lexer.Scan(source)).ParseExpression();
    private static List<Statement> ParseProgram(string source) => new Parser(Lexer.Scan(source)).ParseProgram();

    [Fact]
    public void MultiplicationBindsTighterThanAddition()
    {
        var expected = new BinaryExpression(
            new NumberExpression(1),
            TokenType.Plus,
            new BinaryExpression(new NumberExpression(2), TokenType.Star, new NumberExpression(3)));
        Assert.Equal(expected, ParseExpr("1 + 2 * 3"));
    }

    [Fact]
    public void SubtractionIsLeftAssociative()
    {
        var expected = new BinaryExpression(
            new BinaryExpression(new NumberExpression(10), TokenType.Minus, new NumberExpression(2)),
            TokenType.Minus,
            new NumberExpression(3));
        Assert.Equal(expected, ParseExpr("10 - 2 - 3"));
    }

    [Fact]
    public void ParsesVariableExpression()
    {
        Assert.Equal(new VariableExpression("x"), ParseExpr("x"));
    }

    [Fact]
    public void ParsesLetAndPrintStatements()
    {
        var program = ParseProgram("let x = 5; print x;");
        Assert.Equal(2, program.Count);
        Assert.Equal(new LetStatement("x", new NumberExpression(5)) { Line = 1 }, program[0]);
        Assert.Equal(new PrintStatement(new VariableExpression("x")) { Line = 1 }, program[1]);
    }

    [Theory]
    [InlineData("let x = 5")]
    [InlineData("let = 5;")]
    [InlineData("print (1 + 2;")]
    [InlineData("5;")]
    public void ThrowsOnInvalidSyntax(string source)
    {
        Assert.Throws<ParseError>(() => ParseProgram(source));
    }

    [Fact]
    public void ParsesStringLiteral()
    {
        Assert.Equal(new StringExpression("hi"), ParseExpr("\"hi\""));
    }

    [Fact]
    public void StatementsRememberTheirLine()
    {
        var program = ParseProgram("let x = 1;\n\nprint x;");
        Assert.Equal(1, program[0].Line);
        Assert.Equal(3, program[1].Line);
    }

    [Fact]
    public void ParseErrorReportsLineNumber()
    {
        var error = Assert.Throws<ParseError>(() => ParseProgram("let x = 1;\nlet y = 2\nprint y;"));
        Assert.Equal(3, error.Line);
        Assert.Equal("[line 3] Expected ';' after expression", error.Message);
    }
}
