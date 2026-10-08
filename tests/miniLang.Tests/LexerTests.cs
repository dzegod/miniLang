using miniLang;

namespace miniLang.Tests;

public class LexerTests
{
    private static List<TokenType> Types(string source) => Lexer.Scan(source).Select(t => t.Type).ToList();

    [Fact]
    public void ScansArithmetic()
    {
        Assert.Equal(
            new List<TokenType> { TokenType.Number, TokenType.Plus, TokenType.LParen, TokenType.Number, TokenType.Star, TokenType.Number, TokenType.RParen, TokenType.Eof },
            Types("1 + (2 * 3)"));
    }

    [Fact]
    public void ScansKeywordsAndIdentifiers()
    {
        Assert.Equal(
            new List<TokenType> { TokenType.Let, TokenType.Identifier, TokenType.Equal, TokenType.Number, TokenType.Semicolon, TokenType.Print, TokenType.Identifier, TokenType.Semicolon, TokenType.Eof },
            Types("let x = 5; print x;"));
    }

    [Fact]
    public void ScansComparisonOperators()
    {
        Assert.Equal(
            new List<TokenType> { TokenType.EqualEqual, TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual, TokenType.Eof },
            Types("== < <= > >="));
    }

    [Fact]
    public void ScansStringLiteral()
    {
        var token = Lexer.Scan("\"hello\"")[0];
        Assert.Equal(TokenType.String, token.Type);
        Assert.Equal("hello", token.Lexeme);
    }

    [Fact]
    public void ThrowsOnUnterminatedString()
    {
        Assert.Throws<LexError>(() => Lexer.Scan("\"oops"));
    }

    [Fact]
    public void ThrowsOnUnexpectedCharacter()
    {
        Assert.Throws<LexError>(() => Lexer.Scan("1 @ 2"));
    }

    [Fact]
    public void TokensRememberTheirLine()
    {
        var tokens = Lexer.Scan("let x = 1;\nprint x;");
        Assert.Equal(1, tokens[0].Line);
        Assert.Equal(2, tokens[5].Line);
    }

    [Fact]
    public void LexErrorReportsLineNumber()
    {
        var error = Assert.Throws<LexError>(() => Lexer.Scan("1;\n2;\n3 @ 4;"));
        Assert.Equal(3, error.Line);
    }
}
