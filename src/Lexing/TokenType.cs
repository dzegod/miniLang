namespace miniLang;

public enum TokenType
{
    Number, Identifier,
    Plus, Minus, Star, Slash,
    LParen, RParen,
    LBrace, RBrace,
    Equal, Semicolon, EqualEqual,
    Less, LessEqual, Greater, GreaterEqual,
    Let, Print, If, Else, While, True, False,
    String,
    Eof
}