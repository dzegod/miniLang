namespace miniLang;

public class LexError : Exception
{
    public LexError(string message) : base(message) { }
}
