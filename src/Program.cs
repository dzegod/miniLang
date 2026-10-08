using miniLang;

var source = """
    let name = "miniLang";
    print "Hello from " + name + "!";

    let i = 1;
    while (i <= 3) {
        print "i = " + i;
        i = i + 1;
    }

    print y;
    """;

try
{
    var tokens = Lexer.Scan(source);
    var parser = new Parser(tokens);
    var program = parser.ParseProgram();

    new Interpreter().Interpret(program);
}
catch (Exception error) when (error is LexError or ParseError or RuntimeError)
{
    Console.WriteLine($"{error.GetType().Name}: {error.Message}");
}
