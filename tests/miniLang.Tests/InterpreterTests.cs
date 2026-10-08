using miniLang;

namespace miniLang.Tests;

public class InterpreterTests
{
    private static object Eval(string source) => new Interpreter().Evaluate(new Parser(Lexer.Scan(source)).ParseExpression());

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

    [Fact]
    public void BlockVariableDisappearsAfterBlock()
    {
        Assert.Throws<RuntimeError>(() => Run("{ let y = 2; } print y;"));
    }

    [Fact]
    public void AssignmentInsideBlockChangesOuterVariable()
    {
        Assert.Equal("10", Run("let i = 0; { i = i + 10; } print i;"));
    }

    [Fact]
    public void WhileLoopRepeatsUntilConditionIsFalse()
    {
        Assert.Equal("0\n1\n2", Run("let i = 0; while (i < 3) { print i; i = i + 1; }"));
    }

    [Theory]
    [InlineData("print \"hello\";", "hello")]
    [InlineData("print \"x = \" + 5;", "x = 5")]
    [InlineData("let name = \"Ana\"; print \"Hi \" + name;", "Hi Ana")]
    [InlineData("print \"a\" == \"a\";", "1")]
    [InlineData("print \"1\" == 1;", "0")]
    [InlineData("if (\"\") { print 1; } else { print 0; }", "0")]
    public void SupportsStrings(string source, string expected)
    {
        Assert.Equal(expected, Run(source));
    }

    [Fact]
    public void ThrowsWhenSubtractingStrings()
    {
        Assert.Throws<RuntimeError>(() => Run("print \"a\" - 1;"));
    }

    [Fact]
    public void RuntimeErrorReportsLineNumber()
    {
        var error = Assert.Throws<RuntimeError>(() => Run("let x = 1;\nprint x;\nprint y;"));
        Assert.Equal(3, error.Line);
        Assert.Equal("[line 3] Undefined variable: y", error.Message);
    }

    [Fact]
    public void RuntimeErrorInsideLoopReportsInnerLine()
    {
        var error = Assert.Throws<RuntimeError>(() => Run("let i = 0;\nwhile (i < 1) {\n  i = i / 0;\n}"));
        Assert.Equal(3, error.Line);
    }
}
