using miniLang;

var tokens = Lexer.Scan("let i = 0; while (i < 3) { print i; i = i + 1; }");
var parser = new Parser(tokens);
var program = parser.ParseProgram();

new Interpreter().Interpret(program);