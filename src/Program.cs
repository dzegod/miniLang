using miniLang;

string[] tests = { "10 - 2 - 3", "2 * (3 + 4)", "20 / 4 / 5", "(2 + 3) * (4 - 1)" };

foreach (var test in tests)
{
    var tokens = Lexer.Scan(test);
    var parser = new Parser(tokens);
    var expr = parser.ParseExpression();
    var result = new Interpreter().Evaluate(expr);
    Console.WriteLine($"{test} = {result}");
}