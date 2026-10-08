# miniLang
Savo skriptinės kalbos interpretatorius C# (.NET), tree-walking.
Struktūra: Lexer.cs (tekstas → Token), Parser.cs (Token → AST, recursive descent),
Ast.cs (Expression/Statement record'ai), Interpreter.cs (vykdo AST).
Parseris: ParseExpression → ParseTerm → ParsePrimary (precedence per kvietimų grandinę).
Padaryta: skaičiai, + - * /, skliaustai, kintamieji (let), print.
Dar nėra: if/else, while, { } blokai, string'ai Parser'yje, klaidų su eilutės numeriu.
Taisyklė: aiškinti kodą, kad suprasčiau, nerašyti visko už mane.