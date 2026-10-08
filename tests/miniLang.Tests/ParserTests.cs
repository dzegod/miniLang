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
        Assert.Equal(new LetStatement("x", new NumberExpression(5)), program[0]);
        Assert.Equal(new PrintStatement(new VariableExpression("x")), program[1]);
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
}
