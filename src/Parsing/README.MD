# miniLang
Savo skriptinės kalbos interpretatorius C# (.NET), tree-walking.
Struktūra: Lexer.cs (tekstas → Token), Parser.cs (Token → AST, recursive descent),
Ast.cs (Expression/Statement record'ai), Interpreter.cs (vykdo AST).
Parseris: ParseExpression → ParseComparison → ParseAddition → ParseTerm → ParsePrimary (precedence per kvietimų grandinę).
Padaryta: skaičiai, string'ai, + - * /, palyginimai (< <= > >= ==), skliaustai, kintamieji (let, priskyrimas x = ...), print, { } blokai su scope, if/else, while, klaidos su eilutės numeriu.
Dar nėra: true/false (bool tipas), funkcijos, komentarai, != operatorius, skaičiai su kableliu.
Taisyklė: aiškinti kodą, kad suprasčiau, nerašyti visko už mane.