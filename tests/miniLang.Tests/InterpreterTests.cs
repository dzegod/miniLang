using miniLang;

namespace miniLang.Tests;

public class InterpreterTests
{
    private static double Eval(string source) => new Interpreter().Evaluate(new Parser(Lexer.Scan(source)).ParseExpression());

    private static string Run(string source)
    {
        var output = new StringWriter();
        var original = Console.Out;
        Console.SetOut(output);
        try
        {
            new Interpreter().Interpret(new Parser(Lexer.Scan(source)).ParseProgram());
        }
        finally
        {
            Console.SetOut(original);
        }
        return output.ToString().Trim();
    }

    [Theory]
    [InlineData("10 - 2 - 3", 5)]
    [InlineData("2 * (3 + 4)", 14)]
    [InlineData("20 / 4 / 5", 1)]
    [InlineData("(2 + 3) * (4 - 1)", 15)]
    public void EvaluatesArithmetic(string source, double expected)
    {
        Assert.Equal(expected, Eval(source));
    }

    [Fact]
    public void RunsProgramWithVariables()
    {
        Assert.Equal("15", Run("let x = 5; let y = x * 2; print x + y;"));
    }

    [Fact]
    public void ThrowsOnUndefinedVariable()
    {
        Assert.Throws<RuntimeError>(() => Run("print z;"));
    }

    [Fact]
    public void ThrowsOnDivisionByZero()
    {
        Assert.Throws<RuntimeError>(() => Eval("1 / 0"));
    }
}
