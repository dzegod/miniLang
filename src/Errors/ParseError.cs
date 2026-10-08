namespace miniLang;

public class ParseError : Exception
{
    public ParseError(string message) : base(message) { }
}
