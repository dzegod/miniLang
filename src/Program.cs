using miniLang;

var tokens = Lexer.Scan("print 3 < 5; print 2 + 2 == 4; print 10 <= 3;");
var parser = new Parser(tokens);
var program = parser.ParseProgram();

new Interpreter().Interpret(program);